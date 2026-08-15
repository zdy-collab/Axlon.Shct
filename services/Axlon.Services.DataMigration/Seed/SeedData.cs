using Axlon.Services.Contracts.Merchant;
using Axlon.Services.Contracts.Merchant.Enums;
using Axlon.Services.Contracts.Models;
using SqlSugar;

namespace Axlon.Services.DataMigration.Seed;

public sealed class UserInfoSeedData : IIdempotentEntitySeedData<SysUserInfo>
{
    public IReadOnlyList<string> KeyColumns => [nameof(SysUserInfo.Id)];

    public IEnumerable<SysUserInfo> InitSeedData() =>
    [
        new SysUserInfo("user", "E10ADC3949BA59ABBE56E057F20F883E")
        {
            Id = 10000, Name = "user", RealName = "测试用户", Enable = true, TenantId = 0
        },
        new SysUserInfo("blogadmin", "3FACF26687DAB7254848976256EDB56F")
        {
            Id = 10001, Name = "blogadmin", RealName = "超级管理员", Enable = true, TenantId = 0
        },
        new SysUserInfo("test", "E10ADC3949BA59ABBE56E057F20F883E")
        {
            Id = 10002, Name = "test", RealName = "测试用户2", Enable = true, TenantId = 0
        }
    ];

    public IEnumerable<SysUserInfo> SeedData() => [];

    public Task CustomizeSeedData(ISqlSugarClient db) => Task.CompletedTask;
}

public sealed class RoleSeedData : IIdempotentEntitySeedData<Role>
{
    public IReadOnlyList<string> KeyColumns => [nameof(Role.Id)];

    public IEnumerable<Role> InitSeedData() =>
    [
        new Role("SuperAdmin")
        {
            Id = 10000, Description = "超级管理员", Enabled = true, OrderSort = 1, IsDeleted = false, AuthorityScope = 9
        },
        new Role("Admin")
        {
            Id = 10001, Description = "管理员", Enabled = true, OrderSort = 2, IsDeleted = false, AuthorityScope = 3
        },
        new Role("System")
        {
            Id = 10002, Description = "系统用户", Enabled = true, OrderSort = 3, IsDeleted = false, AuthorityScope = 2
        },
        new Role("Client")
        {
            Id = 10003, Description = "客户端用户", Enabled = true, OrderSort = 4, IsDeleted = false, AuthorityScope = 4
        }
    ];

    public IEnumerable<Role> SeedData() => [];

    public Task CustomizeSeedData(ISqlSugarClient db) => Task.CompletedTask;
}

public sealed class UserRoleSeedData : IIdempotentEntitySeedData<UserRole>
{
    public IReadOnlyList<string> KeyColumns => [nameof(UserRole.UserId), nameof(UserRole.RoleId)];

    public IEnumerable<UserRole> InitSeedData() =>
    [
        new UserRole(10000, 10000),
        new UserRole(10001, 10001),
        new UserRole(10002, 10003)
    ];

    public IEnumerable<UserRole> SeedData() => [];

    public Task CustomizeSeedData(ISqlSugarClient db) => Task.CompletedTask;
}

public sealed class DepartmentSeedData : IIdempotentEntitySeedData<Department>
{
    public IReadOnlyList<string> KeyColumns => [nameof(Department.Id)];

    public IEnumerable<Department> InitSeedData() =>
    [
        new Department
        {
            Id = 10000, Name = "总公司", Pid = 0, CodeRelationship = "", OrderSort = 1, Status = true, IsDeleted = false
        },
        new Department
        {
            Id = 10001, Name = "研发部", Pid = 10000, CodeRelationship = "10000,", OrderSort = 1, Status = true, IsDeleted = false
        },
        new Department
        {
            Id = 10002, Name = "运维部", Pid = 10000, CodeRelationship = "10000,", OrderSort = 2, Status = true, IsDeleted = false
        }
    ];

    public IEnumerable<Department> SeedData() => [];

    public Task CustomizeSeedData(ISqlSugarClient db) => Task.CompletedTask;
}


/// <summary>
/// 初始化商户
/// </summary>
public sealed class MerchantSeedData : IIdempotentEntitySeedData<Merchants>
{
    public IReadOnlyList<string> KeyColumns => [nameof(Merchants.Id)];

    public IEnumerable<Merchants> InitSeedData() =>
    [

                //new Merchants()
                //{
                //    Name = "张三的川菜馆",
                //    Address = "重庆市光电园1",
                //    Longitude = 106.504355M,
                //    Latitude = 29.614755M,
                //    GeoHash = "wum3v9y7",
                //    //BusinessHours = "9:00-18:00",
                //    Status = MerchantStatus.已通过,
                //},
                //new Merchants()
                //{
                //    Name = "李四美发沙龙",
                //    Address = "重庆市光电园2",
                //    Longitude = 106.504272M,
                //    Latitude = 29.614226M,
                //    GeoHash = "wum3v9y7",
                //    //BusinessHours = "9:00-18:00",
                //    Status = MerchantStatus.已通过,
                //},
                //new Merchants()
                //{
                //    Name = "北京故宫",
                //    Address = "北京",
                //    Longitude = 116.397029M,
                //    Latitude = 39.917732M,
                //    GeoHash = "wx4g0cp8",
                //    //BusinessHours = "9:00-18:00",
                //    Status = MerchantStatus.已通过,
                //}

    ];

    public IEnumerable<Merchants> SeedData() => [];

    public Task CustomizeSeedData(ISqlSugarClient db) => Task.CompletedTask;
}