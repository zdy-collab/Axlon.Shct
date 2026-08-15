using Axlon.Framework.Abstractions;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Files.OutInput;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files.Controllers;

[NonController]
[Authorize]
[Route("api/files/oss")]
public sealed class OssFilesController(IFileTransferApplication files) : BaseApiController
{
    private const string ProviderName = "oss";

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = 2_148_532_224)]
    public async Task<ActionResult<MessageModel<FileMetadataOutput>>> Upload(
        [FromForm] FileTransferRequest request,
        CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        await using var content = request.File.OpenReadStream();
        var output = await files.UploadAsync(
            ProviderName,
            request.File.FileName,
            request.File.Length,
            request.Visibility,
            content,
            cancellationToken);
        return Created($"/api/files/oss/{output.Id}/preview", Success(output, "File uploaded."));
    }

    [HttpGet("{id:long}/download")]
    public Task<ActionResult> Download(
        [Range(1, long.MaxValue)] long id,
        CancellationToken cancellationToken) =>
        RedirectToFileAsync(id, inline: false, cancellationToken);

    [HttpGet("{id:long}/preview")]
    public Task<ActionResult> Preview(
        [Range(1, long.MaxValue)] long id,
        CancellationToken cancellationToken) =>
        RedirectToFileAsync(id, inline: true, cancellationToken);

    private async Task<ActionResult> RedirectToFileAsync(
        long id,
        bool inline,
        CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        var output = await files.CreateAccessUrlAsync(ProviderName, id, inline, cancellationToken);
        return Redirect(output.Url);
    }
}
