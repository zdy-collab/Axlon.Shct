using Axlon.Services.Files.ObjectStorage;
using Axlon.Services.Files.ObjectStorage.AliyunOss;
using Axlon.Services.Files.ObjectStorage.Local;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Axlon.Services.Files;

internal static class FilesServiceCollectionExtensions
{
    public static IServiceCollection AddFilesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<FilesOptions>()
            .Bind(configuration.GetSection(FilesOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.DefaultProvider), "DefaultProvider is required.")
            .Validate(options => options.MaxImageBytes > 0 && options.MaxVideoBytes >= options.MaxImageBytes,
                "File size limits are invalid.")
            .Validate(options => options.MultipartThresholdBytes > 0
                && options.PartSizeBytes >= 100 * 1024
                && options.PartSizeBytes <= 5L * 1024 * 1024 * 1024,
                "Multipart settings are invalid.")
            .Validate(options => options.SignedUrlLifetimeMinutes is >= 1 and <= 60,
                "SignedUrlLifetimeMinutes must be between 1 and 60.")
            .Validate(options => options.UploadSessionLifetimeHours is >= 1 and <= 168,
                "UploadSessionLifetimeHours must be between 1 and 168.")
            .Validate(options => options.DeletedRetentionDays is >= 1 and <= 365,
                "DeletedRetentionDays must be between 1 and 365.")
            .Validate(options => options.CleanupIntervalMinutes > 0
                && options.CleanupBatchSize is >= 1 and <= 1000
                && options.CleanupLeaseMinutes > 0,
                "Cleanup settings are invalid.")
            .ValidateOnStart();

        services.AddSingleton(provider => provider.GetRequiredService<IOptions<FilesOptions>>().Value);
        services.AddSingleton(TimeProvider.System);
        services.AddLocalObjectStorage(configuration);

        var configuredDefault = configuration[$"{FilesOptions.SectionName}:DefaultProvider"] ?? "local";
        var ossSection = configuration.GetSection("Axlon:Files:Providers:oss");
        var ossEnabled = ossSection.GetValue<bool?>("Enabled")
            ?? configuredDefault.Equals("oss", StringComparison.OrdinalIgnoreCase);
        if (ossEnabled)
            services.AddAliyunOss(configuration);
        else if (configuredDefault.Equals("oss", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The default OSS provider is disabled.");

        services.AddSingleton<IObjectStorageProviderResolver, ObjectStorageProviderResolver>();
        services.AddScoped<IFileObjectStore, SqlFileObjectStore>();
        services.AddScoped<IFileApplication, FileApplication>();
        services.AddScoped<IFileTransferApplication, FileTransferApplication>();
        services.AddExceptionHandler<FilesExceptionHandler>();
        services.AddHostedService<FileCleanupWorker>();
        services.AddHealthChecks().AddCheck<FilesHealthCheck>("files-dependencies", tags: ["ready"]);
        return services;
    }
}
