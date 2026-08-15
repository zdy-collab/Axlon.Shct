using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Axlon.Services.Files.ObjectStorage.Local;

public static class LocalObjectStorageExtensions
{
    public static IServiceCollection AddLocalObjectStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<LocalObjectStorageOptions>()
            .Bind(configuration.GetSection(LocalObjectStorageOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ProviderName), "Local ProviderName is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Bucket), "Local Bucket is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.RootPath), "Local RootPath is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.UploadPath), "Local UploadPath is required.")
            .Validate(options => string.IsNullOrWhiteSpace(options.PublicBaseUrl)
                || Uri.TryCreate(options.PublicBaseUrl, UriKind.Absolute, out _),
                "Local PublicBaseUrl must be an absolute URI when configured.")
            .ValidateOnStart();

        services.AddDataProtection().SetApplicationName("Axlon.Services.Files.LocalStorage");
        services.AddHttpContextAccessor();
        services.AddSingleton<LocalObjectStorageProvider>();
        services.AddSingleton<IObjectStorageProvider>(provider =>
            provider.GetRequiredService<LocalObjectStorageProvider>());
        services.AddSingleton<IObjectStorageDataPlane>(provider =>
            provider.GetRequiredService<LocalObjectStorageProvider>());
        return services;
    }
}
