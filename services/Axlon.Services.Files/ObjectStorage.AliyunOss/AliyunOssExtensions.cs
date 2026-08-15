using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace Axlon.Services.Files.ObjectStorage.AliyunOss;

public static class AliyunOssExtensions
{
    public static IServiceCollection AddAliyunOss(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<AliyunOssOptions>()
            .Bind(configuration.GetSection(AliyunOssOptions.SectionName))
            .Configure(options => ApplyEnvironmentOverrides(options))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ProviderName),
                $"{AliyunOssOptions.SectionName}:ProviderName is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Region),
                $"{AliyunOssOptions.SectionName}:Region is required.")
            .Validate(options => Uri.TryCreate(options.Endpoint, UriKind.Absolute, out var endpoint)
                && endpoint.Scheme == Uri.UriSchemeHttps,
                $"{AliyunOssOptions.SectionName}:Endpoint must be an absolute HTTPS URI.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Bucket),
                $"{AliyunOssOptions.SectionName}:Bucket is required.")
            .Validate(options => CredentialsAreComplete(options),
                $"{AliyunOssOptions.SectionName}:AccessKeyId and AccessKeySecret must both be set or both be omitted.")
            .ValidateOnStart();

        services.AddSingleton<IObjectStorageProvider, AliyunOssProvider>();
        services.AddHttpClient(AliyunOssDataPlane.HttpClientName, client =>
            client.Timeout = Timeout.InfiniteTimeSpan);
        services.AddSingleton<IObjectStorageDataPlane, AliyunOssDataPlane>();
        return services;
    }

    private static bool CredentialsAreComplete(AliyunOssOptions options)
    {
        var configured = !string.IsNullOrWhiteSpace(options.AccessKeyId)
            && !string.IsNullOrWhiteSpace(options.AccessKeySecret);
        var environment = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OSS_ACCESS_KEY_ID"))
            && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OSS_ACCESS_KEY_SECRET"));
        return configured || environment;
    }

    private static void ApplyEnvironmentOverrides(AliyunOssOptions options)
    {
        options.Region = Environment.GetEnvironmentVariable("OSS_REGION") ?? options.Region;
        options.Endpoint = Environment.GetEnvironmentVariable("OSS_ENDPOINT") ?? options.Endpoint;
        options.Bucket = Environment.GetEnvironmentVariable("OSS_BUCKET") ?? options.Bucket;
        if (bool.TryParse(Environment.GetEnvironmentVariable("OSS_USE_CNAME"), out var useCName))
            options.UseCName = useCName;
        options.AccessKeyId = Environment.GetEnvironmentVariable("OSS_ACCESS_KEY_ID") ?? options.AccessKeyId;
        options.AccessKeySecret = Environment.GetEnvironmentVariable("OSS_ACCESS_KEY_SECRET") ?? options.AccessKeySecret;
        options.SecurityToken = Environment.GetEnvironmentVariable("OSS_SESSION_TOKEN") ?? options.SecurityToken;
    }
}
