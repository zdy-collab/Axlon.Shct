using System;

namespace Axlon.Services.Files;

public sealed class FileServiceException(int statusCode, string code, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public int StatusCode { get; } = statusCode;
    public string Code { get; } = code;

    public static FileServiceException BadRequest(string message) => new(400, "invalid_request", message);
    public static FileServiceException Forbidden(string message = "You do not have access to this file.") => new(403, "forbidden", message);
    public static FileServiceException NotFound(string message = "File not found.") => new(404, "not_found", message);
    public static FileServiceException Conflict(string message) => new(409, "conflict", message);
    public static FileServiceException Gone(string message) => new(410, "gone", message);
    public static FileServiceException TooLarge(string message) => new(413, "file_too_large", message);
    public static FileServiceException Unsupported(string message) => new(415, "unsupported_media", message);
    public static FileServiceException BadGateway(string message, Exception? exception = null) =>
        new(502, "storage_provider_error", message, exception);
    public static FileServiceException StorageUnavailable(string message, Exception? exception = null) =>
        new(503, "storage_unavailable", message, exception);
}
