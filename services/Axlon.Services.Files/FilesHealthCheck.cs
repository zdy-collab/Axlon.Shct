using Axlon.Services.Files.ObjectStorage;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SqlSugar;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Axlon.Services.Files;

internal sealed class FilesHealthCheck(
    ISqlSugarClient database,
    IObjectStorageProviderResolver providers,
    IOptions<FilesOptions> options) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            _ = await database.Ado.GetIntAsync("SELECT 1");
            await providers.Resolve(options.Value.DefaultProvider).CheckHealthAsync(cancellationToken);
            return HealthCheckResult.Healthy("filesdb and the default object storage provider are reachable.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Files dependencies are unavailable.", exception);
        }
    }
}
