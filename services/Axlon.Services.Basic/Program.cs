using Axlon.Framework.Authentication.ServiceExtensions;
using Axlon.Framework.Core.DependencyInjection;
using Axlon.Framework.Data.DependencyInjection;
using Axlon.Framework.EventBus;
using Axlon.Framework.Redis;
using Axlon.Framework.Serilog.Extensions;
using Axlon.Framework.Web.DependencyInjection;
using Axlon.Services.Basic.External;
using Axlon.Services.Basic.ServiceInvocation.File;
using Axlon.Services.Contracts.Base.CommonEnum;
using Axlon.Services.Contracts.BdGeography;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.Wechat;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = true;
    options.ValidateScopes = true;
});
builder.AddServiceDefaults();
builder.AddAxlonSerilog();
builder.AddAxlonCore();
builder.Services.AddAxlonSqlSugar(builder.Configuration, typeof(SysUserInfo).Assembly);
builder.Services.AddAxlonRepositories(typeof(Program).Assembly);
builder.Services.AddAxlonApplication(typeof(Program).Assembly);
builder.Services.AddAxlonHybridCache(builder.Configuration);
builder.Services.AddAxlonSecurity(builder.Configuration);
builder.Services.AddAxlonWebApi(builder.Configuration);
builder.Services.AddAxlonRedis(builder.Configuration);
builder.Services.AddAxlonCap(builder.Configuration, builder.Environment, "basic");

builder.Services.AddHttpClient("auth",
    client => client.BaseAddress = new Uri("https+http://auth"));
builder.Services.AddHttpClient("files",
    client => client.BaseAddress = new Uri("https+http://files"));
builder.Services.AddHttpClient(ServiceName.wechat.ToString(), 
    c => c.BaseAddress = new Uri("https://api.weixin.qq.com"));
builder.Services.AddHttpClient(ServiceName.bdGeography.ToString(), 
    c => c.BaseAddress = new Uri("https://api.map.baidu.com"));

builder.Services.Configure<WechatOptions>(builder.Configuration.GetSection("Wechat"));
builder.Services.Configure<BdGeographyOptions>(builder.Configuration.GetSection("Baidu"));


var app = builder.Build();
app.MapDefaultEndpoints();
app.UseAxlonWebApi();
app.UseAxlonCapDashboardAudit();
app.Run();

public partial class Program;
