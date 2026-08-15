using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files.ObjectStorage.Local;

internal sealed class LocalObjectStorageProvider : IObjectStorageProvider, IObjectStorageDataPlane
{
    private const int BufferSize = 128 * 1024;
    private const int MaximumRangeLength = 1024 * 1024;
    private static readonly JsonSerializerOptions TokenJson = new(JsonSerializerDefaults.Web);
    private static readonly IReadOnlyDictionary<string, string> ContentTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp",
            [".gif"] = "image/gif",
            [".mp4"] = "video/mp4",
            [".webm"] = "video/webm",
            [".mov"] = "video/quicktime"
        };

    private readonly LocalObjectStorageOptions _options;
    private readonly IDataProtector _tokens;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TimeProvider _timeProvider;
    private readonly string _rootPath;
    private readonly string _uploadPath;
    private readonly StringComparison _pathComparison = OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;

    public LocalObjectStorageProvider(
        IOptions<LocalObjectStorageOptions> options,
        IDataProtectionProvider dataProtection,
        IWebHostEnvironment environment,
        IHttpContextAccessor httpContextAccessor,
        TimeProvider timeProvider)
    {
        _options = options.Value;
        _tokens = dataProtection.CreateProtector("Axlon.Services.Files.ObjectStorage.Local;.v1");
        _httpContextAccessor = httpContextAccessor;
        _timeProvider = timeProvider;
        _rootPath = ResolveConfiguredPath(environment.ContentRootPath, _options.RootPath);
        _uploadPath = ResolveConfiguredPath(environment.ContentRootPath, _options.UploadPath);
        Directory.CreateDirectory(_rootPath);
        Directory.CreateDirectory(_uploadPath);
    }

    public string Name => _options.ProviderName;
    public string Bucket => _options.Bucket;
    public string ProviderName => Name;

    public Task<ObjectUploadSession> InitializeUploadAsync(
        ObjectUploadIntent intent,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureAddress(intent.Address);
        if (intent.Mode == ObjectUploadMode.Single)
            return Task.FromResult(new ObjectUploadSession(null));

        var uploadId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
        Directory.CreateDirectory(GetSessionPath(uploadId));
        return Task.FromResult(new ObjectUploadSession(uploadId));
    }

    public Task<SignedObjectRequest> SignUploadAsync(
        ObjectUploadIntent intent,
        string? providerUploadId,
        int? partNumber,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureAddress(intent.Address);
        if (lifetime <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(lifetime));

        if (intent.Mode == ObjectUploadMode.Single)
        {
            if (providerUploadId is not null || partNumber is not null)
                throw new ArgumentException("Single uploads cannot include multipart identifiers.");
        }
        else
        {
            _ = GetSessionPath(providerUploadId ?? throw new ArgumentException("Multipart upload ID is required."));
            if (partNumber is < 1 or > 10_000)
                throw new ArgumentOutOfRangeException(nameof(partNumber));
        }

        var expiration = _timeProvider.GetUtcNow().Add(lifetime);
        var token = Protect(new LocalSignedToken
        {
            Operation = LocalTokenOperations.Upload,
            Provider = intent.Address.Provider,
            Bucket = intent.Address.Bucket,
            ObjectKey = intent.Address.ObjectKey,
            ExpiresAt = expiration,
            ContentType = NormalizeContentType(intent.ContentType),
            ContentLength = intent.ContentLength,
            ProviderUploadId = providerUploadId,
            PartNumber = partNumber
        });

        return Task.FromResult(new SignedObjectRequest(
            BuildUrl("/api/file-uploads/local/", token),
            expiration,
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Content-Type"] = NormalizeContentType(intent.ContentType)
            }));
    }

    public async Task CompleteUploadAsync(
        ObjectUploadIntent intent,
        string providerUploadId,
        IReadOnlyList<CompletedObjectPart> parts,
        CancellationToken cancellationToken = default)
    {
        EnsureAddress(intent.Address);
        var finalPath = ResolveObjectPath(intent.Address);
        if (File.Exists(finalPath))
        {
            if (new FileInfo(finalPath).Length != intent.ContentLength)
                throw new InvalidOperationException("The completed local object has an unexpected length.");
            DeleteDirectoryIfPresent(GetSessionPath(providerUploadId));
            return;
        }

        var sessionPath = GetSessionPath(providerUploadId);
        if (!Directory.Exists(sessionPath))
            throw new FileNotFoundException("The local multipart upload session does not exist.");
        if (parts.Count == 0)
            throw new ArgumentException("Multipart completion requires at least one part.", nameof(parts));

        Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);
        var temporaryPath = finalPath + "." + Guid.NewGuid().ToString("N") + ".merging";
        long totalLength = 0;
        try
        {
            await using (var output = CreateWriteStream(temporaryPath))
            {
                for (var index = 0; index < parts.Count; index++)
                {
                    var part = parts[index];
                    if (part.PartNumber != index + 1)
                        throw new ArgumentException("Multipart parts must be unique and contiguous.", nameof(parts));

                    var partPath = GetPartPath(sessionPath, part.PartNumber);
                    var etagPath = GetPartEtagPath(sessionPath, part.PartNumber);
                    if (!File.Exists(partPath) || !File.Exists(etagPath))
                        throw new FileNotFoundException($"Local multipart part {part.PartNumber} is missing.");

                    var storedEtag = (await File.ReadAllTextAsync(etagPath, cancellationToken)).Trim();
                    if (!storedEtag.Equals(part.ETag.Trim(), StringComparison.Ordinal))
                        throw new InvalidOperationException($"ETag for local multipart part {part.PartNumber} does not match.");

                    await using var input = new FileStream(partPath, FileMode.Open, FileAccess.Read, FileShare.Read,
                        BufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
                    totalLength = checked(totalLength + input.Length);
                    if (totalLength > intent.ContentLength)
                        throw new InvalidOperationException("Multipart data exceeds the declared object length.");
                    await input.CopyToAsync(output, BufferSize, cancellationToken);
                }
                await output.FlushAsync(cancellationToken);
            }
            if (totalLength != intent.ContentLength)
                throw new InvalidOperationException("Multipart data does not match the declared object length.");

            File.Move(temporaryPath, finalPath, overwrite: false);
            DeleteDirectoryIfPresent(sessionPath);
        }
        finally
        {
            DeleteFileIfPresent(temporaryPath);
        }
    }

    public Task AbortUploadAsync(
        ObjectAddress address,
        string providerUploadId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureAddress(address);
        DeleteDirectoryIfPresent(GetSessionPath(providerUploadId));
        return Task.CompletedTask;
    }

    public Task<ObjectInfo?> GetInfoAsync(
        ObjectAddress address,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = ResolveObjectPath(address);
        if (!File.Exists(path)) return Task.FromResult<ObjectInfo?>(null);
        var contentType = GetContentType(path);
        return Task.FromResult<ObjectInfo?>(new ObjectInfo(new FileInfo(path).Length, contentType, null));
    }

    public async Task<byte[]> ReadRangeAsync(
        ObjectAddress address,
        long offset,
        int length,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        if (length is <= 0 or > MaximumRangeLength)
            throw new ArgumentOutOfRangeException(nameof(length));

        var path = ResolveObjectPath(address);
        await using var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            BufferSize, FileOptions.Asynchronous | FileOptions.RandomAccess);
        if (offset >= input.Length) return [];
        input.Seek(offset, SeekOrigin.Begin);
        var count = (int)Math.Min(length, input.Length - offset);
        var buffer = new byte[count];
        await input.ReadExactlyAsync(buffer, cancellationToken);
        return buffer;
    }

    public Task<SignedObjectRequest> SignDownloadAsync(
        ObjectAddress address,
        string fileName,
        ObjectDownloadDisposition disposition,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _ = ResolveObjectPath(address);
        if (lifetime <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(lifetime));
        var expiration = _timeProvider.GetUtcNow().Add(lifetime);
        var token = Protect(new LocalSignedToken
        {
            Operation = LocalTokenOperations.Download,
            Provider = address.Provider,
            Bucket = address.Bucket,
            ObjectKey = address.ObjectKey,
            ExpiresAt = expiration,
            FileName = Path.GetFileName(fileName),
            Disposition = disposition
        });
        return Task.FromResult(new SignedObjectRequest(
            BuildUrl("/api/files/local/", token), expiration,
            new Dictionary<string, string>()));
    }

    public Task DeleteAsync(ObjectAddress address, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        DeleteFileIfPresent(ResolveObjectPath(address));
        return Task.CompletedTask;
    }

    public async Task CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_rootPath);
        Directory.CreateDirectory(_uploadPath);
        var probe = Path.Combine(_uploadPath, ".health-" + Guid.NewGuid().ToString("N"));
        try
        {
            await File.WriteAllBytesAsync(probe, [0x1], cancellationToken);
        }
        finally
        {
            DeleteFileIfPresent(probe);
        }
    }

    internal async Task<string> WriteSignedUploadAsync(
        string token,
        Stream body,
        string? requestContentType,
        long? requestContentLength,
        CancellationToken cancellationToken = default)
    {
        var signed = Unprotect(token, LocalTokenOperations.Upload);
        var expectedLength = signed.ContentLength
            ?? throw new LocalObjectStorageRequestException(StatusCodes.Status403Forbidden, "Invalid upload token.");
        if (requestContentLength != expectedLength)
            throw new LocalObjectStorageRequestException(StatusCodes.Status400BadRequest,
                "Content-Length does not match the signed upload request.");
        if (!NormalizeContentType(requestContentType).Equals(signed.ContentType, StringComparison.OrdinalIgnoreCase))
            throw new LocalObjectStorageRequestException(StatusCodes.Status400BadRequest,
                "Content-Type does not match the signed upload request.");

        var address = new ObjectAddress(signed.Provider, signed.Bucket, signed.ObjectKey);
        EnsureAddress(address);
        if (signed.PartNumber is null)
            return await WriteSingleAsync(address, body, expectedLength, cancellationToken);

        var sessionPath = GetSessionPath(signed.ProviderUploadId
            ?? throw new LocalObjectStorageRequestException(StatusCodes.Status403Forbidden, "Invalid multipart token."));
        if (!Directory.Exists(sessionPath))
            throw new LocalObjectStorageRequestException(StatusCodes.Status410Gone,
                "The multipart upload session has expired.");
        return await WritePartAsync(sessionPath, signed.PartNumber.Value, body, expectedLength, cancellationToken);
    }

    public Task<string> UploadAsync(
        SignedObjectRequest request,
        Stream content,
        string contentType,
        long contentLength,
        CancellationToken cancellationToken = default)
    {
        // 只接受本 adapter 自己签发的数据端点，避免把外部 URL 当成本地文件路径处理。
        const string routePrefix = "/api/file-uploads/local/";
        var marker = request.Url.AbsolutePath.IndexOf(routePrefix, StringComparison.OrdinalIgnoreCase);
        if (marker < 0)
            throw new InvalidOperationException("The signed request was not issued by the local provider.");

        var token = Uri.UnescapeDataString(request.Url.AbsolutePath[(marker + routePrefix.Length)..]);
        if (string.IsNullOrWhiteSpace(token) || token.Contains('/'))
            throw new InvalidOperationException("The local upload token is invalid.");

        return WriteSignedUploadAsync(token, content, contentType, contentLength, cancellationToken);
    }

    internal LocalDownloadFile ResolveSignedDownload(string token)
    {
        var signed = Unprotect(token, LocalTokenOperations.Download);
        var address = new ObjectAddress(signed.Provider, signed.Bucket, signed.ObjectKey);
        var path = ResolveObjectPath(address);
        if (!File.Exists(path))
            throw new LocalObjectStorageRequestException(StatusCodes.Status404NotFound, "File not found.");
        return new LocalDownloadFile(
            path,
            GetContentType(path),
            Path.GetFileName(signed.FileName ?? Path.GetFileName(path)),
            signed.Disposition ?? ObjectDownloadDisposition.Attachment);
    }

    private async Task<string> WriteSingleAsync(
        ObjectAddress address,
        Stream body,
        long expectedLength,
        CancellationToken cancellationToken)
    {
        var finalPath = ResolveObjectPath(address);
        if (File.Exists(finalPath))
            throw new LocalObjectStorageRequestException(StatusCodes.Status409Conflict, "The local object already exists.");
        Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);
        var temporaryPath = finalPath + "." + Guid.NewGuid().ToString("N") + ".uploading";
        try
        {
            var etag = await WriteStreamAsync(temporaryPath, body, expectedLength, cancellationToken);
            try { File.Move(temporaryPath, finalPath, overwrite: false); }
            catch (IOException) when (File.Exists(finalPath))
            {
                throw new LocalObjectStorageRequestException(StatusCodes.Status409Conflict,
                    "The local object already exists.");
            }
            return etag;
        }
        finally
        {
            DeleteFileIfPresent(temporaryPath);
        }
    }

    private async Task<string> WritePartAsync(
        string sessionPath,
        int partNumber,
        Stream body,
        long expectedLength,
        CancellationToken cancellationToken)
    {
        if (partNumber is < 1 or > 10_000)
            throw new LocalObjectStorageRequestException(StatusCodes.Status403Forbidden, "Invalid part number.");
        var partPath = GetPartPath(sessionPath, partNumber);
        var temporaryPath = partPath + "." + Guid.NewGuid().ToString("N") + ".uploading";
        try
        {
            var etag = await WriteStreamAsync(temporaryPath, body, expectedLength, cancellationToken);
            File.Move(temporaryPath, partPath, overwrite: true);
            await File.WriteAllTextAsync(GetPartEtagPath(sessionPath, partNumber), etag, cancellationToken);
            return etag;
        }
        finally
        {
            DeleteFileIfPresent(temporaryPath);
        }
    }

    private static async Task<string> WriteStreamAsync(
        string targetPath,
        Stream source,
        long expectedLength,
        CancellationToken cancellationToken)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        long total = 0;
        try
        {
            await using var output = CreateWriteStream(targetPath);
            while (true)
            {
                var read = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
                if (read == 0) break;
                total = checked(total + read);
                if (total > expectedLength)
                    throw new LocalObjectStorageRequestException(StatusCodes.Status413PayloadTooLarge,
                        "Upload body exceeds the signed length.");
                hash.AppendData(buffer, 0, read);
                await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }
            await output.FlushAsync(cancellationToken);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        if (total != expectedLength)
            throw new LocalObjectStorageRequestException(StatusCodes.Status400BadRequest,
                "Upload body does not match the signed length.");
        return $"\"{Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant()}\"";
    }

    private LocalSignedToken Unprotect(string token, string expectedOperation)
    {
        try
        {
            var json = _tokens.Unprotect(token);
            var value = JsonSerializer.Deserialize<LocalSignedToken>(json, TokenJson)
                ?? throw new CryptographicException("Token payload is empty.");
            if (!value.Operation.Equals(expectedOperation, StringComparison.Ordinal)
                || value.ExpiresAt <= _timeProvider.GetUtcNow())
                throw new CryptographicException("Token is invalid or expired.");
            EnsureAddress(new ObjectAddress(value.Provider, value.Bucket, value.ObjectKey));
            return value;
        }
        catch (LocalObjectStorageRequestException) { throw; }
        catch (Exception exception) when (exception is CryptographicException or JsonException or FormatException)
        {
            throw new LocalObjectStorageRequestException(StatusCodes.Status403Forbidden,
                "The local storage token is invalid or expired.");
        }
    }

    private string Protect(LocalSignedToken value) =>
        _tokens.Protect(JsonSerializer.Serialize(value, TokenJson));

    private Uri BuildUrl(string path, string token)
    {
        var relativePath = path + Uri.EscapeDataString(token);
        if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
            return new Uri(new Uri(_options.PublicBaseUrl.TrimEnd('/') + "/"), relativePath.TrimStart('/'));

        var request = _httpContextAccessor.HttpContext?.Request
            ?? throw new InvalidOperationException("Local PublicBaseUrl is required outside an HTTP request.");
        var origin = $"{request.Scheme}://{request.Host}{request.PathBase}".TrimEnd('/');
        return new Uri(origin + relativePath, UriKind.Absolute);
    }

    private void EnsureAddress(ObjectAddress address)
    {
        if (!Name.Equals(address.Provider, StringComparison.OrdinalIgnoreCase)
            || !Bucket.Equals(address.Bucket, StringComparison.Ordinal))
            throw new InvalidOperationException("Object address does not belong to this local provider.");
        _ = ResolveObjectPathCore(address.ObjectKey);
    }

    private string ResolveObjectPath(ObjectAddress address)
    {
        if (!Name.Equals(address.Provider, StringComparison.OrdinalIgnoreCase)
            || !Bucket.Equals(address.Bucket, StringComparison.Ordinal))
            throw new InvalidOperationException("Object address does not belong to this local provider.");
        return ResolveObjectPathCore(address.ObjectKey);
    }

    private string ResolveObjectPathCore(string objectKey)
    {
        if (string.IsNullOrWhiteSpace(objectKey)
            || objectKey.StartsWith('/')
            || objectKey.EndsWith('/')
            || objectKey.Contains('\\'))
            throw new InvalidOperationException("The local object key is invalid.");
        var segments = objectKey.Split('/');
        if (segments.Length == 0 || segments.Any(segment => string.IsNullOrWhiteSpace(segment)
            || segment is "." or ".."
            || segment.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
            throw new InvalidOperationException("The local object key is invalid.");
        var path = Path.GetFullPath(Path.Combine([_rootPath, .. segments]));
        EnsureWithinRoot(path, _rootPath);
        return path;
    }

    private string GetSessionPath(string uploadId)
    {
        if (!Guid.TryParseExact(uploadId, "N", out _))
            throw new InvalidOperationException("The local upload ID is invalid.");
        var path = Path.GetFullPath(Path.Combine(_uploadPath, uploadId));
        EnsureWithinRoot(path, _uploadPath);
        return path;
    }

    private void EnsureWithinRoot(string path, string root)
    {
        var prefix = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        if (!path.StartsWith(prefix, _pathComparison))
            throw new InvalidOperationException("The resolved local storage path escapes its configured root.");
    }

    private static string GetPartPath(string sessionPath, int partNumber) =>
        Path.Combine(sessionPath, partNumber.ToString("D5", CultureInfo.InvariantCulture) + ".part");

    private static string GetPartEtagPath(string sessionPath, int partNumber) =>
        Path.Combine(sessionPath, partNumber.ToString("D5", CultureInfo.InvariantCulture) + ".etag");

    private static FileStream CreateWriteStream(string path) =>
        new(path, FileMode.CreateNew, FileAccess.Write, FileShare.None,
            BufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);

    private static string ResolveConfiguredPath(string contentRoot, string configuredPath) =>
        Path.GetFullPath(Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(contentRoot, configuredPath));

    private static string NormalizeContentType(string? value) =>
        value?.Split(';', 2)[0].Trim().ToLowerInvariant() ?? string.Empty;

    private static string GetContentType(string path) =>
        ContentTypes.TryGetValue(Path.GetExtension(path), out var contentType)
            ? contentType
            : "application/octet-stream";

    private static void DeleteFileIfPresent(string path)
    {
        try { File.Delete(path); }
        catch (FileNotFoundException) { }
        catch (DirectoryNotFoundException) { }
    }

    private static void DeleteDirectoryIfPresent(string path)
    {
        try { Directory.Delete(path, recursive: true); }
        catch (DirectoryNotFoundException) { }
    }
}
