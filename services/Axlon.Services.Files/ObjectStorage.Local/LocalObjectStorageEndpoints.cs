using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files.ObjectStorage.Local;

public static class LocalObjectStorageEndpoints
{
    public static IApplicationBuilder UseLocalObjectStorageLogRedaction(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            try
            {
                await next(context);
            }
            finally
            {
                // Serilog 在下游返回后读取 Request.Path；此时隐藏 bearer token，避免签名地址进入日志。
                var path = context.Request.Path.Value;
                if (path?.StartsWith("/api/file-uploads/local/", StringComparison.OrdinalIgnoreCase) == true)
                    context.Request.Path = "/api/file-uploads/local/{token}";
                else if (path?.StartsWith("/api/files/local/", StringComparison.OrdinalIgnoreCase) == true)
                    context.Request.Path = "/api/files/local/{token}";
            }
        });

    public static IEndpointRouteBuilder MapLocalObjectStorageEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/file-uploads/local/{token}", UploadAsync)
            .AllowAnonymous()
            .DisableRateLimiting()
            .WithMetadata(new DisableRequestSizeLimitAttribute())
            .ExcludeFromDescription();

        endpoints.MapGet("/api/files/local/{token}", Download)
            .AllowAnonymous()
            .DisableRateLimiting()
            .ExcludeFromDescription();
        return endpoints;
    }

    private static async Task<IResult> UploadAsync(
        string token,
        HttpContext context,
        LocalObjectStorageProvider storage,
        ILogger<LocalObjectStorageProvider> logger,
        CancellationToken cancellationToken)
    {
        var requestLimit = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
        if (requestLimit is { IsReadOnly: false }) requestLimit.MaxRequestBodySize = null;

        try
        {
            var etag = await storage.WriteSignedUploadAsync(
                token,
                context.Request.Body,
                context.Request.ContentType,
                context.Request.ContentLength,
                cancellationToken);
            context.Response.Headers.ETag = etag;
            context.Response.Headers.Append(
                Microsoft.Net.Http.Headers.HeaderNames.AccessControlExposeHeaders,
                Microsoft.Net.Http.Headers.HeaderNames.ETag);
            return Results.NoContent();
        }
        catch (LocalObjectStorageRequestException exception)
        {
            return Results.Problem(statusCode: exception.StatusCode, title: exception.Message);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            logger.LogError(exception, "Local object upload failed.");
            return Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Local storage is unavailable.");
        }
    }

    private static IResult Download(
        string token,
        HttpContext context,
        LocalObjectStorageProvider storage,
        ILogger<LocalObjectStorageProvider> logger)
    {
        try
        {
            var download = storage.ResolveSignedDownload(token);
            var disposition = new ContentDispositionHeaderValue(
                download.Disposition == ObjectDownloadDisposition.Inline ? "inline" : "attachment")
            {
                FileName = ToAsciiFileName(download.FileName),
                FileNameStar = download.FileName
            };
            context.Response.Headers.ContentDisposition = disposition.ToString();
            context.Response.Headers.CacheControl = "private, no-store";
            return Results.File(download.PhysicalPath, download.ContentType, enableRangeProcessing: true);
        }
        catch (LocalObjectStorageRequestException exception)
        {
            return Results.Problem(statusCode: exception.StatusCode, title: exception.Message);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            logger.LogError(exception, "Local object download failed.");
            return Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Local storage is unavailable.");
        }
    }

    private static string ToAsciiFileName(string fileName)
    {
        var sanitized = new string(fileName.Select(character =>
            character is >= (char)0x20 and <= (char)0x7e && character is not '"' and not '\\'
                ? character
                : '_').ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? "download" : sanitized;
    }
}
