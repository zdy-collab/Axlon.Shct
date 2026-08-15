using Axlon.Framework.Web;
using Axlon.Framework.Web.ServiceExtensions;
using Axlon.Services.Category.Seed;
using Axlon.Services.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// 1. Aspire Service Defaults (OpenTelemetry traces/metrics + HealthChecks + ServiceDiscovery).
//    Serilog continues to handle application logging via UseAxlonFramework below.
builder.AddServiceDefaults();

// 1b. HttpClient for forwarding Swagger login to the Auth service. Resolves "http://auth"
//     via Aspire service discovery (injected by AppHost WithReference) or falls back to
//     the Services:auth:http entry in appsettings.json when running standalone.
builder.Services.AddHttpClient("auth-service", c =>
{
    c.BaseAddress = new Uri("http://auth");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddServiceDiscovery();

// 2. Override DBS connection strings with the Aspire-injected PostgreSQL connection.
var pgConn = builder.Configuration.GetConnectionString("axlondb");
var pgLogConn = builder.Configuration.GetConnectionString("axlondb.log");

if (!string.IsNullOrEmpty(pgConn))
{
    builder.Configuration["DBS:0:Connection"] = pgConn;
    builder.Configuration["DBS:2:Connection"] = pgLogConn; // Log 库共享同一 PostgreSQL 实例
}

// 3. Framework one-shot registration (Autofac + SqlSugar + JWT authentication + Swagger + ...).
builder.Host.UseAxlonFramework<AutofacModuleRegister>(opts =>
{
    opts.ServiceDllName = "Axlon.Services.Category.dll";
    opts.RepositoryDllName = "Axlon.Services.Category.dll";
    opts.XmlDocFiles = Array.Empty<string>();
    opts.AuthPolicyName = "Permission";
    opts.EnableLogHub = false;
    opts.SwaggerThemeStylesheet = "/swagger-ui/tech-theme.css";
    // 共享契约程序集（RoleModulePermission/Permission/Modules/SysTenant 等实体定义在此），
    // 需声明扫描前缀，使 CodeFirst 建表和 Autofac 注册能发现这些实体。
    opts.AdditionalAssemblyPrefixes = new[] { "Axlon.Services.Contracts" };
});

// 4. RBAC authorization registration (PermissionHandler + PermissionRequirement policy).
//    Basic 服务需要完整 RBAC 权限校验:SuperAdmin 绕过 + URL 正则匹配。
//builder.Services.AddAxlonRbacAuthorization<PermissionHandler>();

builder.Services.AddScoped<DBSeed>();

var app = builder.Build();

//app.UseCors("CorsIpAccess");

// 5. Aspire default endpoints (/health, /alive). Must be mapped before UseAxlonFramework.
app.MapDefaultEndpoints();

// 6. Framework middleware pipeline (UseAuthentication/UseAuthorization/UseRouting/MapControllers).
app.UseAxlonFramework();

app.Run();
