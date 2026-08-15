# Axlon.Shct

基于 **.NET 10 + .NET Aspire** 的微服务后端项目，提供认证授权与基础管理（RBAC）能力。从 `Axlon.Framework.SampleApi` 演进而来，重新托管为 Aspire 编排的微服务架构。

## 目录

- [项目简介](#项目简介)
- [技术栈](#技术栈)
- [解决方案结构](#解决方案结构)
- [架构设计](#架构设计)
- [快速开始](#快速开始)
- [配置说明](#配置说明)
- [构建与发布](#构建与发布)
- [贡献指南](#贡献指南)

---

## 项目简介

Axlon.Shct 是一个企业级微服务模板，核心包含两大业务服务：

- **Auth 服务**：登录、JWT 签发、Token 刷新、注销与 Swagger 登录。所有登录接口 `[AllowAnonymous]`，不走 RBAC。
- **Basic 服务**：用户 / 角色 / 权限 / 模块 / 部门 / 用户角色 CRUD，带完整 RBAC 权限校验（SuperAdmin 绕过 + URL 正则匹配）。

两个服务共享同一份契约（实体、DTO、ViewModel）和同一套 JWT 参数，由 Aspire AppHost 统一编排 PostgreSQL、Redis 及服务实例。

## 技术栈

| 维度 | 选型 |
|------|------|
| 运行时 | .NET 10 (SDK `10.0.301`) |
| 编排 | .NET Aspire `13.4.6`（含 Docker Compose 环境） |
| 数据库 | PostgreSQL（主库 `axlondb` + 日志库 `axlonLogdb`） |
| 缓存 | Redis（已编排，当前 `Redis:Enable=false`） |
| ORM | SqlSugarCore（CodeFirst 建表 + 多库支持） |
| 认证 | JWT Bearer（共享 Secret/Issuer/Audience） |
| 授权 | 自定义 `PermissionHandler` + `PermissionRequirement`（RBAC） |
| 日志 | Serilog（文件 / 数据库 / Elasticsearch） |
| 可观测性 | OpenTelemetry（Traces + Metrics，OTLP 导出） |
| 对象映射 | Mapster |
| DI 容器 | Autofac（模块化扫描注册） |
| 配置中心 | Nacos（可选） |
| 混淆 | BitMono（Lite 模式，发布时可选启用） |
| 业务框架 | `Axlon.Framework.*`（本地 NuGet 源 `artifacts/`） |

## 解决方案结构

```
Axlon.Shct/
├── Axlon.Shct.slnx                   # 解决方案（新 .slnx 格式）
├── global.json                       # .NET SDK 10.0.301 + Aspire SDK
├── nuget.config                      # nuget.org + 本地 artifacts 源
├── aspire/                           # Aspire 编排层
│   ├── Axlon.AppHost/                # 编排入口（PostgreSQL + Redis + Auth + Basic）
│   └── Axlon.Services.ServiceDefaults/  # 共享 OTel / HealthChecks / ServiceDiscovery
├── services/                         # 业务服务
│   ├── Axlon.Services.Auth/          # 认证服务（登录 / JWT / 刷新 / 注销）
│   ├── Axlon.Services.Basic/         # 基础管理服务（RBAC CRUD）
│   └── Axlon.Services.Contracts/     # 跨服务共享实体 / DTO / ViewModel
├── build/                            # 构建脚本
│   ├── pack-all.ps1                  # 打包 Axlon.Framework.* 系列 NuGet
│   ├── obfuscate-bitmono.ps1         # BitMono 混淆钩子
│   └── bitmono.yml                   # 混淆规则（Lite 模式，保留公共 API）
└── artifacts/                        # 本地 NuGet 包（Axlon.Framework.*）
```

## 架构设计

### 整体拓扑

```
┌──────────────────────────┐
                    │   Axlon.AppHost (Aspire) │
                    │   编排入口 + 参数注入     │
                    └──────────┬───────────────┘
                               │
        ┌──────────────────────┼──────────────────────┐
        │                      │                      │
   ┌────▼─────┐          ┌─────▼─────┐         ┌──────▼──────┐
   │ PostgreSQL│          │   Redis   │         │   (Params)  │
   │ 5433      │          │ (未启用)  │         │ jwt-secret  │
   │ PgWeb:5050│          └───────────┘         │ jwt-issuer  │
   └────┬──────┘                                │ jwt-audience│
        │                                       └──────┬──────┘
        │ ConnectionStrings:axlondb / axlondb.log     │
        ├──────────────────────┬──────────────────────┘
        │                      │
   ┌────▼──────────┐     ┌─────▼──────────┐
   │ Auth 服务     │     │ Basic 服务     │
   │ /api/auth/*   │◄────┤ /api/user/*    │
   │ (AllowAnonymous)│   │ (RBAC 校验)    │
   └───────────────┘     └────────────────┘
        │                      │
        └──────────┬───────────┘
                   │
           ┌───────▼────────┐
           │ Axlon.Services │
           │  .Contracts    │  共享实体 / DTO / ViewModel
           └────────────────┘
```

### 服务内部分层（Auth / Basic 一致）

```
Controllers/         # API 端点
   └─ BaseApiController（来自 Axlon.Framework.Web）
IServices/           # 业务接口
Services/            # 业务实现（Autofac 自动注册）
Repository/          # 数据访问，继承 BaseRepository<T>（SqlSugar）
Authorization/       # PermissionHandler + RbacAuthorizationSetup
MapsterConfig/       # 对象映射配置
Seed/                # CodeFirst 种子数据（IEntitySeedData<T>）
Program.cs           # UseAxlonFramework<AutofacModuleRegister> 一站式注册
```

### 认证授权流程

1. Auth 服务 `/api/auth/login` 校验账号密码（MD5）后签发 JWT，Claims 包含 `Name`、`Jti`、`TenantId`、`Iat`、`Expiration`、`Role`。
2. 客户端携带 JWT 请求 Basic 服务。
3. `PermissionHandler` 解析 Token，校验：用户存在 / 未删除 / 未禁用 / Token 未过期 / `CriticalModifyTime` 早于签发时间。
4. SuperAdmin 直接放行；其他角色按 `RoleModulePermission` 表中的 `Module.LinkUrl` 与当前请求路径做正则匹配。
5. 通过则 `context.Succeed(requirement)`，否则返回 401 并写入 `_user.Message`。

### 共享 JWT 参数

AppHost 通过 `AddParameter` 注入三个参数，Aspire 9.0+ 在开发态写入 user-secrets，运行态注入到两个服务的配置中，确保 Auth 签发的 Token 在 Basic 同样有效：

```csharp
var jwtSecret   = builder.AddParameter("jwt-secret", secret: true);
var jwtIssuer   = builder.AddParameter("jwt-issuer", "Axlon.Framework");
var jwtAudience = builder.AddParameter("jwt-audience", "wr");
```

## 快速开始

### 前置依赖

- [.NET SDK 10.0.301+](https://dotnet.microsoft.com/download)
- Docker Desktop（用于 Aspire 拉起 PostgreSQL / Redis 容器）
- （可选）`BITMONO_PATH` 环境变量，用于发布时混淆

### 启动

```bash
# 1. 还原依赖（包含本地 artifacts/ 源中的 Axlon.Framework.*）
dotnet restore

# 2. 以 Aspire AppHost 为入口启动（会自动拉起 PG / Redis / Auth / Basic）
dotnet run --project aspire/Axlon.AppHost
```

启动成功后：

- Aspire Dashboard：控制台输出的 URL（默认 `http://localhost:18888`）
- PostgreSQL：`localhost:5433`，账号 `postgres / 123456`
- PgWeb（DB 管理）：`http://localhost:5050`
- Auth 服务 Swagger：Aspire Dashboard 中查看 `auth` 项目分配的端口
- Basic 服务 Swagger：Aspire Dashboard 中查看 `basic` 项目分配的端口

### 默认种子数据

数据库启动时由 `Seed/DBSeed.cs` + 各 `IEntitySeedData<T>` 自动建表并写入种子数据（部门：总公司 / 研发部 / 运维部 等）。

## 配置说明

每个服务下的 `appsettings.json` 共享相同结构，关键节点：

| 节点 | 说明 |
|------|------|
| `DBS` | 多数据库连接配置，`DBType: 4` 表示 PostgreSQL；Aspire 启动时 `ConnectionStrings:axlondb` 会覆盖 `DBS:0:Connection` |
| `MainDB` | 主库 ConnId，默认 `Main` |
| `Audience` | JWT 签发参数（Secret / Issuer / Audience） |
| `AppSettings` | 框架行为开关：`SeedDBEnabled`、`CachingAOP`、`LogAOP`、`TranAOP`、`SqlAOP` 等 |
| `Middleware` | 中间件开关：请求日志、IP 日志、访问日志、SignalR、Quartz、Consul、IpRateLimit、加解密 |
| `IpRateLimiting` | 接口限流：默认 `1s/3`、`1m/30`、`12h/500` |
| `Startup` | CORS、ApiName、IdentityServer4 / Authing / Nacos 开关 |
| `Redis` / `RabbitMQ` / `Kafka` / `EventBus` | 可选中间件，默认全部 `false` |

Basic 服务额外包含 `Services:auth:http`，用于独立运行（非 Aspire）时通过 HttpClient 转发 Swagger 登录到 Auth 服务。

## 构建与发布

### 还原与编译

```bash
dotnet build Axlon.Shct.slnx -c Release
```

默认 NuGet 源：`http://baget.axlon.internal/v3/index.json`

## 贡献指南

1. Fork 本仓库
2. 新建特性分支：`git checkout -b Feat_xxx`
3. 提交代码并推送：`git commit -m "feat: xxx"` → `git push origin Feat_xxx`
4. 发起 Pull Request，描述变更范围与测试情况

### 约定

- 新增业务服务时，应同时引用 `Axlon.Services.ServiceDefaults` 与 `Axlon.Services.Contracts`，并通过 `builder.Host.UseAxlonFramework<AutofacModuleRegister>(...)` 完成框架注册。
- 跨服务共享的实体 / DTO 必须放入 `Axlon.Services.Contracts`，不要在业务服务间互相引用。
- 涉及 DB 结构变更的，使用 `IEntitySeedData<T>` 维护种子数据，由 CodeFirst 自动同步。

---

### Git提交注意

- 为了方便后期回滚与排查问题，推崇少量多次推送，不要几个功能冗余在一起推送，需要严格按照以下约定执行：
- 【新增】xxxxx表示新增业务功能
- 【修复】xxxxx表示修复bug或者其他
- 【文档】针对文档的修改
- 【其他】其他重要的提交



