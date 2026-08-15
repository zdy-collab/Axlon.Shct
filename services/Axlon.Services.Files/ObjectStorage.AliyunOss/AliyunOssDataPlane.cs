using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files.ObjectStorage.AliyunOss;

/// <summary>
/// 将控制器收到的文件流发送到 OSS 预签名地址。签名、分片和 ETag 仍由 Files 模块在内部协调。
/// </summary>
internal sealed class AliyunOssDataPlane(
    IHttpClientFactory httpClientFactory,
    IOptions<AliyunOssOptions> options) : IObjectStorageDataPlane
{
    internal const string HttpClientName = "Axlon.ObjectStorage.AliyunOss.Upload";

    public string ProviderName => options.Value.ProviderName;

    public async Task<string> UploadAsync(
        SignedObjectRequest request,
        Stream content,
        string contentType,
        long contentLength,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, request.Url);
        using var body = new StreamContent(content);
        body.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        body.Headers.ContentLength = contentLength;
        message.Content = body;

        // OSS V4 签名包含这些请求头，必须原样发送，否则服务端会判定签名不一致。
        foreach (var header in request.Headers)
        {
            if (!message.Headers.TryAddWithoutValidation(header.Key, header.Value))
                body.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        var client = httpClientFactory.CreateClient(HttpClientName);
        using var response = await client.SendAsync(
            message, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var etag = response.Headers.ETag?.ToString();
        if (string.IsNullOrWhiteSpace(etag)
            && response.Headers.TryGetValues("ETag", out var values))
            etag = values.FirstOrDefault();
        return !string.IsNullOrWhiteSpace(etag)
            ? etag
            : throw new InvalidOperationException("OSS upload response did not contain an ETag.");
    }
}
