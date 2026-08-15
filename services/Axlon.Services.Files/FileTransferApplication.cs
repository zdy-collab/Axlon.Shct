using Axlon.Services.Contracts.Models.Files;
using Axlon.Services.Files.ObjectStorage;
using Axlon.Services.Files.OutInput;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files;

/// <summary>对外隐藏签名、分片和完成确认，只提供上传与访问授权。</summary>
public interface IFileTransferApplication
{
    Task<FileMetadataOutput> UploadAsync(
        string providerName,
        string fileName,
        long length,
        string visibility,
        Stream content,
        CancellationToken cancellationToken);

    Task<DownloadUrlOutput> CreateAccessUrlAsync(
        string providerName,
        long fileId,
        bool inline,
        CancellationToken cancellationToken);
}

internal sealed class FileTransferApplication(
    IFileApplication files,
    IEnumerable<IObjectStorageDataPlane> dataPlanes,
    FilesOptions options,
    ILogger<FileTransferApplication> logger) : IFileTransferApplication
{
    private readonly IReadOnlyDictionary<string, IObjectStorageDataPlane> _dataPlanes =
        dataPlanes.ToDictionary(dataPlane => dataPlane.ProviderName, StringComparer.OrdinalIgnoreCase);

    public async Task<FileMetadataOutput> UploadAsync(
        string providerName,
        string fileName,
        long length,
        string visibility,
        Stream content,
        CancellationToken cancellationToken)
    {
        var dataPlane = ResolveDataPlane(providerName);

        // 浏览器选择文件后无需用户手填 MIME；服务端按允许的扩展名确定 MIME，完成时再用魔数复核内容。
        var contentType = new FileMediaPolicy(options).GetCanonicalContentType(fileName);
        var session = await files.BeginUploadAsync(new BeginFileUploadRequest
        {
            FileName = fileName,
            ContentType = contentType,
            Size = length,
            Visibility = visibility
        }, cancellationToken, providerName);

        try
        {
            var completedParts = session.Mode == FileUploadModes.Multipart
                ? await UploadPartsAsync(dataPlane, session, content, contentType, length, cancellationToken)
                : await UploadSingleAsync(dataPlane, session, content, contentType, length, cancellationToken);

            return await files.CompleteUploadAsync(session.FileId, new CompleteFileUploadRequest
            {
                Parts = completedParts
            }, cancellationToken);
        }
        catch
        {
            // 中途失败时尽力清除临时分片和未完成对象；清理失败不覆盖原始上传异常。
            try
            {
                await files.AbortUploadAsync(session.FileId, CancellationToken.None);
            }
            catch (Exception cleanupException)
            {
                logger.LogWarning(cleanupException, "Failed to clean up direct upload {FileId}", session.FileId);
            }
            throw;
        }
    }

    public Task<DownloadUrlOutput> CreateAccessUrlAsync(
        string providerName,
        long fileId,
        bool inline,
        CancellationToken cancellationToken) =>
        files.CreateDownloadUrlAsync(fileId, new CreateDownloadUrlRequest
        {
            Disposition = inline ? "inline" : "attachment"
        }, cancellationToken, providerName);

    private async Task<CompletedFilePartInput[]> UploadSingleAsync(
        IObjectStorageDataPlane dataPlane,
        BeginFileUploadOutput session,
        Stream content,
        string contentType,
        long length,
        CancellationToken cancellationToken)
    {
        var request = session.Upload
            ?? throw new InvalidOperationException("The single upload session did not return an upload request.");
        await dataPlane.UploadAsync(ToSignedRequest(request), content, contentType, length, cancellationToken);
        return [];
    }

    private async Task<CompletedFilePartInput[]> UploadPartsAsync(
        IObjectStorageDataPlane dataPlane,
        BeginFileUploadOutput session,
        Stream content,
        string contentType,
        long totalLength,
        CancellationToken cancellationToken)
    {
        var partSize = session.PartSize
            ?? throw new InvalidOperationException("The multipart upload session did not return a part size.");
        var partCount = session.PartCount
            ?? throw new InvalidOperationException("The multipart upload session did not return a part count.");
        var completed = new List<CompletedFilePartInput>(partCount);

        for (var firstPart = 1; firstPart <= partCount; firstPart += 50)
        {
            var numbers = Enumerable.Range(firstPart, Math.Min(50, partCount - firstPart + 1)).ToArray();
            var signedParts = await files.SignPartsAsync(session.FileId, new SignFilePartsRequest
            {
                PartNumbers = numbers
            }, cancellationToken);

            foreach (var signedPart in signedParts)
            {
                var offset = checked((long)(signedPart.PartNumber - 1) * partSize);
                var length = Math.Min(partSize, totalLength - offset);
                await using var part = new LengthLimitedReadStream(content, length);
                var etag = await dataPlane.UploadAsync(
                    ToSignedRequest(signedPart.Request), part, contentType, length, cancellationToken);
                completed.Add(new CompletedFilePartInput
                {
                    PartNumber = signedPart.PartNumber,
                    ETag = etag
                });
            }
        }

        return [.. completed];
    }

    private IObjectStorageDataPlane ResolveDataPlane(string providerName) =>
        _dataPlanes.TryGetValue(providerName, out var dataPlane)
            ? dataPlane
            : throw FileServiceException.StorageUnavailable(
                $"Storage provider '{providerName}' is disabled or unavailable.");

    private static SignedObjectRequest ToSignedRequest(SignedRequestOutput request) =>
        new(new Uri(request.Url, UriKind.Absolute), new DateTimeOffset(request.ExpiresAt, TimeSpan.Zero), request.Headers);

    private sealed class LengthLimitedReadStream : Stream
    {
        private readonly Stream _source;
        private readonly long _length;
        private long _remaining;

        public LengthLimitedReadStream(Stream source, long length)
        {
            _source = source;
            _length = length;
            _remaining = length;
        }

        public override bool CanRead => _source.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _length;
        public override long Position
        {
            get => _length - _remaining;
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = _source.Read(buffer, offset, (int)Math.Min(count, _remaining));
            _remaining -= read;
            return read;
        }

        public override async ValueTask<int> ReadAsync(
            Memory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            var read = await _source.ReadAsync(buffer[..(int)Math.Min(buffer.Length, _remaining)], cancellationToken);
            _remaining -= read;
            return read;
        }

        protected override void Dispose(bool disposing) { }
        public override ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
