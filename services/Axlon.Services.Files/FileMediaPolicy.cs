using Axlon.Services.Contracts.Models.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Axlon.Services.Files;

internal sealed record ValidatedMedia(string DisplayName, string Extension, string ContentType, string MediaKind);

internal sealed class FileMediaPolicy(FilesOptions options)
{
    private static readonly IReadOnlyDictionary<string, MediaRule> Rules =
        new Dictionary<string, MediaRule>(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = new("image/jpeg", FileMediaKinds.Image, IsJpeg),
            [".jpeg"] = new("image/jpeg", FileMediaKinds.Image, IsJpeg),
            [".png"] = new("image/png", FileMediaKinds.Image, IsPng),
            [".webp"] = new("image/webp", FileMediaKinds.Image, IsWebP),
            [".gif"] = new("image/gif", FileMediaKinds.Image, IsGif),
            [".mp4"] = new("video/mp4", FileMediaKinds.Video, IsIsoBaseMedia),
            [".webm"] = new("video/webm", FileMediaKinds.Video, IsWebM),
            [".mov"] = new("video/quicktime", FileMediaKinds.Video, IsIsoBaseMedia)
        };

    public string GetCanonicalContentType(string fileName)
    {
        var displayName = Path.GetFileName(fileName.Replace('\\', '/'));
        var extension = Path.GetExtension(displayName).ToLowerInvariant();
        if (!Rules.TryGetValue(extension, out var rule))
            throw FileServiceException.Unsupported("The file extension is not supported.");
        return rule.ContentType;
    }

    public ValidatedMedia ValidateDeclaration(string fileName, string contentType, long size)
    {
        if (size <= 0) throw FileServiceException.BadRequest("File size must be greater than zero.");
        if (string.IsNullOrWhiteSpace(fileName))
            throw FileServiceException.BadRequest("A valid file name is required.");
        if (string.IsNullOrWhiteSpace(contentType))
            throw FileServiceException.Unsupported("A supported Content-Type is required.");

        var displayName = fileName.Replace('\\', '/').Split('/').LastOrDefault()?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length > 255)
            throw FileServiceException.BadRequest("A valid file name is required.");

        var extension = Path.GetExtension(displayName).ToLowerInvariant();
        if (!Rules.TryGetValue(extension, out var rule))
            throw FileServiceException.Unsupported("The file extension is not supported.");

        var normalizedContentType = contentType.Split(';', 2)[0].Trim().ToLowerInvariant();
        if (!rule.ContentType.Equals(normalizedContentType, StringComparison.OrdinalIgnoreCase))
            throw FileServiceException.Unsupported("The file extension and Content-Type do not match.");

        var maximum = rule.MediaKind == FileMediaKinds.Image ? options.MaxImageBytes : options.MaxVideoBytes;
        if (size > maximum)
            throw FileServiceException.TooLarge($"The maximum {rule.MediaKind} size is {maximum} bytes.");

        return new ValidatedMedia(displayName, extension, normalizedContentType, rule.MediaKind);
    }

    public void ValidateStoredObject(FileObject file, long actualSize, string actualContentType, ReadOnlySpan<byte> prefix)
    {
        if (actualSize != file.DeclaredSize)
            throw FileServiceException.Unsupported("The uploaded object size does not match the declared size.");

        var normalizedContentType = actualContentType.Split(';', 2)[0].Trim();
        if (!file.ContentType.Equals(normalizedContentType, StringComparison.OrdinalIgnoreCase))
            throw FileServiceException.Unsupported("The uploaded object Content-Type does not match the declaration.");

        // 扩展名和 MIME 都可由客户端伪造，最终以对象头部魔数作为第三重校验。
        if (!Rules.TryGetValue(file.Extension, out var rule) || !rule.MagicMatches(prefix))
            throw FileServiceException.Unsupported("The uploaded object content does not match its media type.");
    }

    private static bool IsJpeg(ReadOnlySpan<byte> value) =>
        value.Length >= 3 && value[0] == 0xff && value[1] == 0xd8 && value[2] == 0xff;

    private static bool IsPng(ReadOnlySpan<byte> value) =>
        value.StartsWith(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a });

    private static bool IsGif(ReadOnlySpan<byte> value) =>
        value.StartsWith("GIF87a"u8) || value.StartsWith("GIF89a"u8);

    private static bool IsWebP(ReadOnlySpan<byte> value) =>
        value.Length >= 12 && value[..4].SequenceEqual("RIFF"u8) && value.Slice(8, 4).SequenceEqual("WEBP"u8);

    private static bool IsIsoBaseMedia(ReadOnlySpan<byte> value) =>
        value.Length >= 12 && value.Slice(4, 4).SequenceEqual("ftyp"u8);

    private static bool IsWebM(ReadOnlySpan<byte> value) =>
        value.StartsWith(new byte[] { 0x1a, 0x45, 0xdf, 0xa3 });

    private sealed record MediaRule(
        string ContentType,
        string MediaKind,
        Func<ReadOnlySpan<byte>, bool> MagicMatches);
}
