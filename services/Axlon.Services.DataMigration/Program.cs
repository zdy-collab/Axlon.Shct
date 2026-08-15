using Axlon.Framework.Core.DependencyInjection;
using Axlon.Framework.Data.DependencyInjection;
using Axlon.Framework.Serilog.Extensions;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.Models.Files;
using Axlon.Services.DataMigration.Seed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;

var builder = Host.CreateApplicationBuilder(args);
builder.AddAxlonSerilog();
builder.Services.AddAxlonCore(builder.Configuration);
builder.Services.AddAxlonSqlSugar(builder.Configuration, typeof(SysUserInfo).Assembly);

using var host = builder.Build();
var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Axlon.DataMigration");

try
{
    await host.StartAsync();

    var databaseOptions = new AxlonDatabaseOptions();
    builder.Configuration.Bind(databaseOptions);

    var runner = new DatabaseMigrationRunner(
        host.Services.GetRequiredService<ISqlSugarClient>(),
        logger,
        databaseOptions.MainDB,
        typeof(SysUserInfo).Assembly,
        typeof(FileObject).Assembly,
        typeof(DataMigrationAssemblyMarker).Assembly);
    await runner.RunAsync(
        builder.Configuration.GetValue("AppSettings:SeedDBEnabled", true),
        builder.Configuration.GetValue("AppSettings:SeedDBDataEnabled", true));

    logger.LogInformation("Database migration completed successfully.");
    await host.StopAsync();
    return 0;
}
catch (Exception exception)
{
    logger.LogCritical(exception, "Database migration failed.");
    return 1;
}
