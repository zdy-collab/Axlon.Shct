using Axlon.Framework.Authentication.ServiceExtensions;
using Axlon.Framework.Core.DependencyInjection;
using Axlon.Framework.Data.DependencyInjection;
using Axlon.Framework.Serilog.Extensions;
using Axlon.Framework.Web.DependencyInjection;
using Axlon.Services.Auth.Services;
using Axlon.Services.Contracts.Base.CommonEnum;
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
builder.Services.AddAxlonAuthorizationServer<AuthAuthorizationDecisionProvider>();
builder.Services.AddAxlonWebApi(builder.Configuration);

builder.Services.AddHttpClient(ServiceName.wechat.ToString(), c =>
{
    c.BaseAddress = new Uri("https://api.weixin.qq.com");
}).AddServiceDiscovery();

builder.Services.Configure<WechatOptions>(builder.Configuration.GetSection("Wechat"));

var app = builder.Build();
app.MapDefaultEndpoints();
app.MapAxlonAuthorizationDecisionEndpoint();
app.UseAxlonWebApi();
app.Run();

public partial class Program;
