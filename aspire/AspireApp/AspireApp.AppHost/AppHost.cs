using Aspire.Hosting.Docker;
using Aspire.Hosting.Docker.Resources.ComposeNodes;

var builder = DistributedApplication.CreateBuilder(args);
var dashboardOtlpApiKey = builder.AddParameter("dashboard-otlp-api-key", secret: true);

var containerNames = new Dictionary<string, (string EnvironmentVariable, string DefaultName)>
{
    ["compose-dashboard"] = ("DASHBOARD_CONTAINER_NAME", "axlon-dashboard"),
    ["mysql"] = ("MYSQL_CONTAINER_NAME", "axlon-mysql"),
    ["redis"] = ("REDIS_CONTAINER_NAME", "axlon-redis"),
    ["rabbitmq"] = ("RABBITMQ_CONTAINER_NAME", "axlon-rabbitmq"),
    ["gateway"] = ("GATEWAY_CONTAINER_NAME", "axlon-gateway"),
    ["data-migration"] = ("DATA_MIGRATION_CONTAINER_NAME", "axlon-data-migration"),
    ["auth"] = ("AUTH_CONTAINER_NAME", "axlon-auth"),
    ["files"] = ("FILES_CONTAINER_NAME", "axlon-files"),
    ["basic"] = ("BASIC_CONTAINER_NAME", "axlon-basic"),
    ["merchant"] = ("MERCHANT_CONTAINER_NAME", "axlon-merchant"),
    ["order"] = ("ORDER_CONTAINER_NAME", "axlon-order")
};

builder.AddDockerComposeEnvironment("compose")
    .WithDashboard(dashboard => dashboard
        .WithHostPort(6609)
        .WithEnvironment("DASHBOARD__OTLP__AUTHMODE", "ApiKey")
        .WithEnvironment("DASHBOARD__OTLP__PRIMARYAPIKEY", dashboardOtlpApiKey))
    .ConfigureComposeFile(composeFile =>
    {
        foreach (var (serviceName, settings) in containerNames)
        {
            if (composeFile.Services.TryGetValue(serviceName, out var service))
            {
                service.ContainerName = $"${{{settings.EnvironmentVariable}:-{settings.DefaultName}}}";
            }
        }
    })
    .ConfigureEnvFile(environmentVariables =>
    {
        foreach (var settings in containerNames.Values)
        {
            environmentVariables[settings.EnvironmentVariable] = new()
            {
                Name = settings.EnvironmentVariable,
                Description = $"Docker container name (default: {settings.DefaultName})",
                DefaultValue = settings.DefaultName
            };
        }

        environmentVariables["FILES_STORAGE_ROOT"] = new()
        {
            Name = "FILES_STORAGE_ROOT",
            Description = "Persistent storage root for the files service",
            DefaultValue = "/opt/axlon/storage/files"
        };
    });

#pragma warning disable ASPIRECOMPUTE003
var containerRegistry = builder.AddContainerRegistry("nexus", "49.233.152.22:8082");

#region 用户/密码

var mySqlPassword = builder.AddParameter("mysql-password", secret: true);
var redisPassword = builder.AddParameter("redis-password", secret: true);
var rabbitMqUser = builder.AddParameter("rabbitmq-user");
var rabbitMqPassword = builder.AddParameter("rabbitmq-password", secret: true);

#endregion

var mysql = builder.AddMySql("mysql", mySqlPassword)
                   .WithImageTag("8.0")
                   .WithPhpMyAdmin()
                   .WithEndpoint("tcp", endpoint =>
                   {
                       endpoint.Port = 5241;           // 宿主机端口
                       endpoint.IsExternal = true;
                   })
                   //.WithEnvironment("MYSQL_TCP_PORT", "5241")
                   .WithDataVolume()
                   .WithLifetime(ContainerLifetime.Persistent)
                   .PublishAsDockerComposeService((_, service) =>
                   {
                       service.Healthcheck = new()
                       {
                           Test = ["CMD-SHELL", "mysqladmin ping -h 127.0.0.1 -uroot -p\"$$MYSQL_ROOT_PASSWORD\" --silent"],
                           Interval = "5s",
                           Timeout = "3s",
                           Retries = 20,
                           StartPeriod = "10s"
                       };
                   });
var dbMain = mysql.AddDatabase("axlondb");
var filesDb = mysql.AddDatabase("filesdb");

var redis = builder.AddRedis("redis", password: redisPassword)
                   .WithImageTag("8.6")
                   .WithLifetime(ContainerLifetime.Persistent)
                   .WithDataVolume("redis-aicopilot")
                   //.WithHostPort(63791)  // 固定宿主机端口
                   //.WithEnvironment("REDIS_PASSWORD", "123456")  // 固定密码;
                   .WithRedisInsight(x => x.WithImageTag("3.4"));

var rabbitMq = builder.AddRabbitMQ("rabbitmq", rabbitMqUser, rabbitMqPassword)
    .WithManagementPlugin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);


var dataMigration = builder.AddProject<Projects.Axlon_Services_DataMigration>("data-migration")
    .WaitFor(dbMain)
    .WithReference(dbMain)
    .WaitFor(filesDb)
    .WithReference(filesDb)
    .WithEnvironment("DBS__0__Connection", dbMain.Resource.ConnectionStringExpression)
    .WithEnvironment("DBS__1__Connection", filesDb.Resource.ConnectionStringExpression)
    .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", $"x-otlp-api-key={dashboardOtlpApiKey}")
    .WithContainerRegistry(containerRegistry)
    .PublishAsDockerComposeService((_, service) =>
    {
        service.DependsOn!["mysql"].Condition = "service_healthy";
    });
//.PublishAsDockerFile();

var auth = builder.AddProject<Projects.Axlon_Services_Auth>("auth")
    .WithHttpHealthCheck("/health")
    .WaitFor(dbMain)
    .WithReference(dbMain)
    .WithEnvironment("DBS__0__Connection", dbMain.Resource.ConnectionStringExpression)
    .WaitFor(redis)
    .WithReference(redis)
    .WaitForCompletion(dataMigration)
    .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", $"x-otlp-api-key={dashboardOtlpApiKey}")
    .WithContainerRegistry(containerRegistry);

var files = builder.AddProject<Projects.Axlon_Services_Files>("files")
    .WithHttpHealthCheck("/health")
    .WaitFor(filesDb)
    .WithReference(filesDb)
    .WithEnvironment("DBS__0__Connection", filesDb.Resource.ConnectionStringExpression)
    .WaitFor(auth)
    .WithReference(auth)
    .WaitFor(redis)
    .WithReference(redis)
    .WaitForCompletion(dataMigration)
    .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", $"x-otlp-api-key={dashboardOtlpApiKey}")
    .WithContainerRegistry(containerRegistry)
    .PublishAsDockerComposeService((_, service) =>
    {
        ConfigureFilesStorage(service);
    });

var basic = builder.AddProject<Projects.Axlon_Services_Basic>("basic")
    .WithHttpHealthCheck("/health")
    .WaitFor(dbMain)
    .WithReference(dbMain)
    .WithEnvironment("DBS__0__Connection", dbMain.Resource.ConnectionStringExpression)
    .WaitFor(redis)
    .WithReference(redis)
    .WaitFor(rabbitMq)
    .WithReference(rabbitMq)
    .WaitFor(auth)
    .WithReference(auth)
    .WaitFor(files)
    .WithReference(files)
    .WaitForCompletion(dataMigration)
    .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", $"x-otlp-api-key={dashboardOtlpApiKey}")
    .WithContainerRegistry(containerRegistry);



var merchant = builder.AddProject<Projects.Axlon_Services_Merchant>("merchant")
    .WithHttpHealthCheck("/health")
      .WaitFor(redis)
    .WithReference(redis)
    .WaitFor(dbMain)
    .WithReference(dbMain)
    .WithEnvironment("DBS__0__Connection", dbMain.Resource.ConnectionStringExpression)
    .WaitFor(auth)
    .WithReference(auth)
    .WaitFor(basic)
    .WithReference(basic)
    .WaitForCompletion(dataMigration)
    .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", $"x-otlp-api-key={dashboardOtlpApiKey}")
    .WithContainerRegistry(containerRegistry);


var order = builder.AddProject<Projects.Axlon_Services_Order>("order")
    .WithHttpHealthCheck("/health")
     .WaitFor(redis)
    .WithReference(redis)
    .WaitFor(dbMain)
    .WithReference(dbMain)
    .WithEnvironment("DBS__0__Connection", dbMain.Resource.ConnectionStringExpression)
    .WaitFor(auth)
    .WithReference(auth)
    .WaitFor(basic)
    .WithReference(basic)
    .WaitFor(merchant)
    .WithReference(merchant)
    .WaitForCompletion(dataMigration)
    .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", $"x-otlp-api-key={dashboardOtlpApiKey}")
    .WithContainerRegistry(containerRegistry);

var gateway = builder.AddDockerfile("gateway", "Gateway")
    .WithHttpEndpoint(port: 6600, targetPort: 80, name: "http")
    .WithHttpEndpoint(port: 6601, targetPort: 6601, name: "swagger-auth")
    .WithHttpEndpoint(port: 6602, targetPort: 6602, name: "swagger-files")
    .WithHttpEndpoint(port: 6603, targetPort: 6603, name: "swagger-basic")
    .WithHttpEndpoint(port: 6604, targetPort: 6604, name: "swagger-merchant")
    .WithHttpEndpoint(port: 6605, targetPort: 6605, name: "swagger-order")
    .WithExternalHttpEndpoints()
    .WithReference(auth)
    .WithReference(files)
    .WithReference(basic)
    .WithReference(merchant)
    .WithReference(order)
    .WaitFor(auth)
    .WaitFor(files)
    .WaitFor(basic)
    .WaitFor(merchant)
    .WaitFor(order)
    .WithContainerRegistry(containerRegistry)
    .PublishAsDockerComposeService((_, service) =>
        service.Restart = "always");

#pragma warning restore ASPIRECOMPUTE003

builder.Build().Run();

void ConfigureFilesStorage(Service service)
{
    const string root = "${FILES_STORAGE_ROOT}";

    service.AddVolume(new()
    {
        Name = "files-objects",
        Type = "bind",
        Source = $"{root}/objects",
        Target = "/app/wwwroot/files",
        ReadOnly = false
    });

    service.AddVolume(new()
    {
        Name = "files-uploads",
        Type = "bind",
        Source = $"{root}/uploads",
        Target = "/app/wwwroot/.uploads",
        ReadOnly = false
    });
}
