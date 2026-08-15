using Axlon.Framework.Abstractions;
using Axlon.Framework.Web.Controllers;
using Axlon.Services.Contracts.Models.Files;
using Axlon.Services.Files.OutInput;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files.Controllers;

/// <summary>
/// 文件管理
/// </summary>
/// <param name="files"></param>
[ApiController]
[Authorize]
[Route("api/files")]
public sealed class FilesController(IFileTransferApplication files) : BaseApiController
{
    private const string ProviderName = "local";

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
        return Created($"/api/files/local/{output.Id}/preview", Success(output, "File uploaded."));
    }

    /*    /// <summary>
        /// 服务上传文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="visibility"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("internal/upload")]
        [Consumes("application/octet-stream")]
        [AllowAnonymous]
        public async Task<MessageModel<FileMetadataOutput>> InternalUpload(
            CancellationToken cancellationToken,
            [FromQuery] string fileName,
            [FromQuery] string visibility = FileVisibilities.Tenant)
        {
            var output = await files.UploadAsync(
                ProviderName,
                fileName,
                Request.ContentLength ?? 0,
                visibility,
                Request.Body,
                cancellationToken);

            return Success(output);
        }*/

    [HttpGet("{id:long}/download")]
    public Task<ActionResult> Download(
        [Range(1, long.MaxValue)] long id,
        CancellationToken cancellationToken) =>
        RedirectToFileAsync(id, inline: false, cancellationToken);

    [HttpGet("{id:long}/preview")]
    [AllowAnonymous]
    public Task<ActionResult> Preview(
        [Range(1, long.MaxValue)] long id,
        CancellationToken cancellationToken) =>
        RedirectToFileAsync(id, inline: true, cancellationToken);

    [HttpGet("{id:long}/url")]
    [AllowAnonymous]
    public async Task<ActionResult> GetFileUrlAsync(
    long id,
    CancellationToken cancellationToken)
    {
        var output = await files.CreateAccessUrlAsync(
            ProviderName,
            id,
            true,
            cancellationToken);

        return Ok(new
        {
            url = output.Url
        });
    }

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
