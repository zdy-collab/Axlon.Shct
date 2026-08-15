using Axlon.Services.Contracts.Base.CommonEnum;
using SqlSugar;

namespace Axlon.Services.Contracts.Models
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    [SugarTable("SysUserInfo", "用户表")]
    public class SysUserInfo : SysUserInfoRoot<long>
    {
        public static SysUserInfo CreateMiniUser(
            //string loginName,string loginPWD,
            string openId,string unionId,
            string nickname, string Phone, 
            string registerSource, string? avatar, long? registerFromUserId)
        {
            SysUserInfo userInfo = new SysUserInfo 
            {
                //LoginName = loginName,
                //LoginPWD = loginPWD,
                //RealName = loginName,
                Status = 0,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                LastErrorTime = DateTime.Now,
                ErrorCount = 0,
                Name = "",
                OpenId = openId,
                UnionId = unionId,
                Nickname = nickname,
                Phone = Phone,
                PrompterLevel = PrompterLevel.normal,
                FirstOrderCompleted = YesNo.否,
                RegisterSource = registerSource,
                Avatar = avatar,
                RegisterFromUserId = registerFromUserId,
                //CreatedAt = DateTime.Now
            };
            return userInfo;
        }

        public SysUserInfo()
        {
        }

        public SysUserInfo(string loginName, string loginPWD)
        {
            LoginName = loginName;
            LoginPWD = loginPWD;
            RealName = LoginName;
            Status = 0;
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
            LastErrorTime = DateTime.Now;
            ErrorCount = 0;
            Name = "";
        }

        #region sys

        /// <summary>
        /// 登录账号
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "登录账号")]
        public string LoginName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string LoginPWD { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string RealName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long DepartmentId { get; set; } = -1;

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(Length = 2000, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 关键业务修改时间
        /// </summary>
        public DateTime CriticalModifyTime { get; set; } = DateTime.Now;

        /// <summary>
        ///最后异常时间
        /// </summary>
        public DateTime LastErrorTime { get; set; } = DateTime.Now;

        /// <summary>
        ///错误次数
        /// </summary>
        public int ErrorCount { get; set; }


        /// <summary>
        /// 登录账号
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string Name { get; set; }

        // 性别
        [SugarColumn(IsNullable = true)]
        public int Sex { get; set; } = 0;

        // 年龄
        [SugarColumn(IsNullable = true)]
        public int Age { get; set; }

        // 生日
        [SugarColumn(IsNullable = true)]
        public DateTime Birth { get; set; } = DateTime.Now;

        // 地址
        [SugarColumn(Length = 200, IsNullable = true)]
        public string Address { get; set; }

        [SugarColumn(DefaultValue = "1")]
        public bool Enable { get; set; } = true;

        [SugarColumn(IsNullable = true)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [SugarColumn(IsNullable = false, DefaultValue = "0")]
        public long TenantId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(TenantId))]
        public SysTenant Tenant { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<string> RoleNames { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<long> Dids { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string DepartmentName { get; set; }

        #endregion

        #region business
        /// <summary>
        /// 微信openid
        /// </summary>
        [SugarColumn(ColumnName = "openid", Length = 100, IsNullable = true,
            ColumnDescription = "微信openid")]
        public string OpenId { get; set; }


        /// <summary>
        /// 微信unionid
        /// </summary>
        [SugarColumn(ColumnName = "unionid", Length = 100, IsNullable = true,
            ColumnDescription = "微信unionid")]
        public string UnionId { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        [SugarColumn(ColumnName = "nickname", Length = 100, IsNullable = true,
            ColumnDescription = "昵称")]
        public string Nickname { get; set; }


        /// <summary>
        /// 头像URL
        /// </summary>
        [SugarColumn(ColumnName = "avatar", Length = 500, IsNullable = true,
            ColumnDescription = "头像URL")]
        public string Avatar { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        [SugarColumn(ColumnName = "phone", Length = 20, IsNullable = true,
            ColumnDescription = "手机号")]
        public string Phone { get; set; }


        /// <summary>
        /// 推广等级：normal/silver/gold/partner
        /// PrompterLevel
        /// </summary>
        [SugarColumn(ColumnName = "prompter_level", ColumnDataType = "varchar(20)", IsNullable = true,
            DefaultValue = "normal",
            ColumnDescription = "推广等级：normal/silver/gold/partner")]
        public PrompterLevel PrompterLevel { get; set; }


        /// <summary>
        /// 是否完成首单 0否/1是
        /// </summary>
        [SugarColumn(ColumnName = "first_order_completed", IsNullable = false,
            ColumnDataType = "tinyint",
            DefaultValue = "0",
            ColumnDescription = "是否完成首单 0否/1是")]
        public YesNo FirstOrderCompleted { get; set; }


        /// <summary>
        /// 钱包可用余额，冗余字段，实际以user_wallets为准
        /// </summary>
        [SugarColumn(ColumnName = "wallet_balance", ColumnDataType = "decimal(10,2)", IsNullable = true, DefaultValue = "0",
            ColumnDescription = "钱包可用余额，冗余字段，实际以user_wallets为准")]
        public decimal WalletBalance { get; set; }


        /// <summary>
        /// 累计总收益
        /// </summary>
        [SugarColumn(ColumnName = "total_income", ColumnDataType = "decimal(10,2)", IsNullable = true, DefaultValue = "0",
            ColumnDescription = "累计总收益")]
        public decimal TotalIncome { get; set; }


        /// <summary>
        /// 是否允许好友看消费足迹 1是/0否 默认1
        /// </summary>
        [SugarColumn(ColumnName = "is_allow_friend_visible", IsNullable = false, DefaultValue = "1",
            ColumnDescription = "是否允许好友看消费足迹 1是/0否")]
        public YesNo IsAllowFriendVisible { get; set; }


        /// <summary>
        /// 注册来源（推广码/直接访问）
        /// </summary>
        [SugarColumn(ColumnName = "register_source", Length = 50, IsNullable = true,
            ColumnDescription = "注册来源（推广码/直接访问）")]
        public string RegisterSource { get; set; }


        /// <summary>
        /// 注册来源推广人ID → users.id
        /// </summary>
        [SugarColumn(ColumnName = "register_from_user_id", IsNullable = true,
            ColumnDescription = "注册来源推广人ID")]
        public long? RegisterFromUserId { get; set; }


        ///// <summary>
        ///// 创建时间
        ///// </summary>
        //[SugarColumn(ColumnName = "created_at", IsNullable = false,
        //    ColumnDescription = "创建时间")]
        //public DateTime CreatedAt { get; set; }

        #endregion
    }
}
