using Axlon.Services.Contracts.Models;
using Axlon.Services.Contracts.ViewModels;
using Mapster;

namespace Axlon.Services.Basic.AutoMapper
{
    /// <summary>
    /// Mapster 配置
    /// </summary>
    public class CustomProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<SysUserInfo, SysUserInfoDto>()
                .Map(d => d.uID, s => s.Id)
                .Map(d => d.uLoginName, s => s.LoginName)
                .Map(d => d.uLoginPWD, s => s.LoginPWD)
                .Map(d => d.uRealName, s => s.RealName)
                .Map(d => d.uStatus, s => s.Status)
                .Map(d => d.uRemark, s => s.Remark)
                .Map(d => d.uCreateTime, s => s.CreateTime)
                .Map(d => d.uUpdateTime, s => s.UpdateTime)
                .Map(d => d.uLastErrTime, s => s.LastErrorTime)
                .Map(d => d.uErrorCount, s => s.ErrorCount)
                .Map(d => d.name, s => s.Name)
                .Map(d => d.sex, s => s.Sex)
                .Map(d => d.age, s => s.Age)
                .Map(d => d.birth, s => s.Birth)
                .Map(d => d.addr, s => s.Address)
                .Map(d => d.DepartmentId, s => s.DepartmentId)
                .Map(d => d.RoleNames, s => s.RoleNames)
                .Map(d => d.Dids, s => s.Dids)
                .Map(d => d.DepartmentName, s => s.DepartmentName)
                ;

            config.NewConfig<SysUserInfoDto, SysUserInfo>()
                .Map(d => d.LoginName, s => s.uLoginName)
                .Map(d => d.LoginPWD, s => s.uLoginPWD)
                .Map(d => d.RealName, s => s.uRealName)
                .Map(d => d.Status, s => s.uStatus)
                .Map(d => d.Remark, s => s.uRemark)
                .Map(d => d.CreateTime, s => s.uCreateTime)
                .Map(d => d.UpdateTime, s => s.uUpdateTime)
                .Map(d => d.LastErrorTime, s => s.uLastErrTime)
                .Map(d => d.ErrorCount, s => s.uErrorCount)
                .Map(d => d.Name, s => s.name)
                .Map(d => d.Sex, s => s.sex)
                .Map(d => d.Age, s => s.age)
                .Map(d => d.Birth, s => s.birth)
                .Map(d => d.Address, s => s.addr)
                .Map(d => d.DepartmentId, s => s.DepartmentId)
                .Map(d => d.RoleNames, s => s.RoleNames)
                .Map(d => d.Dids, s => s.Dids)
                .Map(d => d.DepartmentName, s => s.DepartmentName)
                ;
        }
    }
}
