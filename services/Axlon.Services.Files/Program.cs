using Axlon.Framework.Authentication.ServiceExtensions;
using Axlon.Framework.Core.DependencyInjection;
using Axlon.Framework.Data.DependencyInjection;
using Axlon.Framework.Serilog.Extensions;
using Axlon.Framework.Web.DependencyInjection;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.Models.Files;
using Axlon.Services.Contracts.Wechat;
using Axlon.Services.Files;
using Axlon.Services.Files.ObjectStorage.Local;
using Axlon.Services.Files.Services;
using Axlon.Services.Files.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = true;
    options.ValidateScopes = true;
});

builder.AddServiceDefaults();
builder.AddAxlonSerilog();
builder.AddAxlonCore();

builder.Services.AddAxlonApplication(typeof(Program).Assembly);
builder.Services.AddAxlonSqlSugar(builder.Configuration, "filesdb", typeof(FileObject).Assembly);
builder.Services.AddAxlonHybridCache(builder.Configuration);
builder.Services.AddFilesModule(builder.Configuration);
builder.Services.AddAxlonSecurity(builder.Configuration);
builder.Services.AddAxlonWebApi(builder.Configuration);
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics.AddMeter(FilesTelemetry.SourceName))
    .WithTracing(tracing => tracing.AddSource(FilesTelemetry.SourceName));
builder.Services.AddHttpClient("auth",
    client => client.BaseAddress = new Uri("https+http://auth"));
builder.Services.AddHttpClient(ServiceName.wechat.ToString(),
    c => c.BaseAddress = new Uri("https://api.weixin.qq.com"));

builder.Services.Configure<WechatOptions>(builder.Configuration.GetSection("Wechat"));

var app = builder.Build();
app.MapDefaultEndpoints();

app.UseAxlonWebApi();
app.UseLocalObjectStorageLogRedaction();
app.MapLocalObjectStorageEndpoints();
app.Run();

public partial class Program;