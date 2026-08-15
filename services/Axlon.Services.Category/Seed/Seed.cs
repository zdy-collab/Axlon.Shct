using Axlon.Services.Contracts.Category;
using Axlon.Services.Contracts.Merchant;
using Axlon.Services.Contracts.Merchant.Enum;
using SqlSugar;

namespace Axlon.Services.Category.Seed
{
    public class DBSeed
    {
        private readonly ISqlSugarClient Context;

        public DBSeed(ISqlSugarClient context)
        {
            //Context = context;
            var scope = context as SqlSugarScope;


            Context = scope.GetConnectionScope("axlon_mysql");
        }

        /// <summary>
        /// 初始化table
        /// </summary>
        public void InitTable()
        {
            Context.CodeFirst.InitTables<Categories>();
        }

        public async Task InitTableData()
        {
            #region Categories 全平台品类树
            await Context.Ado.ExecuteCommandAsync("INSERT INTO categories(id, parent_id, name, level, path, sort, status)" +
                "VALUES(1, 0, '餐饮美食', 1, '/1', 1, 1)," +
                "(2, 1, '火锅', 2, '/1/2', 1, 1)," +
                "(3, 1, '快餐简餐', 2, '/1/3', 2, 1)," +
                "(4, 1, '饮品甜品', 2, '/1/4', 3, 1);");
            #endregion
        }
    }
}
