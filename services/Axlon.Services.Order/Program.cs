
using Axlon.Framework.Authentication.ServiceExtensions;
using Axlon.Framework.Core.DependencyInjection;
using Axlon.Framework.Data.DependencyInjection;
using Axlon.Framework.EventBus;
using Axlon.Framework.Serilog.Extensions;
using Axlon.Framework.Web.DependencyInjection;
using Axlon.Services.Contracts.Models;
using Axlon.Services.Order.ServiceInvocation.Basic;
using Axlon.Services.Order.ServiceInvocation.Merchant;
using Axlon.Services.Order.Services;

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
builder.Services.AddAxlonCap(builder.Configuration, builder.Environment, "order");

builder.Services.AddHttpClient("auth",
    client => client.BaseAddress = new Uri("http://auth"));
builder.Services.AddHttpClient("basic",
    client => client.BaseAddress = new Uri("http://basic"));
builder.Services.AddHttpClient("merchant",
    client => client.BaseAddress = new Uri("http://merchant"));

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseAxlonWebApi();
app.Run();

public partial class Program;
