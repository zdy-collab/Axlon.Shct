using Axlon.Framework.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files;

internal sealed class FilesExceptionHandler(ILogger<FilesExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not FileServiceException fileException) return false;

        if (fileException.StatusCode >= 500)
            logger.LogError(fileException, "Files request failed with {Code}", fileException.Code);
        else
            logger.LogInformation("Files request rejected with {Code}: {Message}", fileException.Code, fileException.Message);

        httpContext.Response.StatusCode = fileException.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(new MessageModel<object?>
        {
            success = false,
            status = fileException.StatusCode,
            msg = fileException.Message,
            response = null
        }, cancellationToken);
        return true;
    }
}
