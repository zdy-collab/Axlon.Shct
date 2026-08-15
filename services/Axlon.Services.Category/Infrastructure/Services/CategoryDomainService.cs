using Axlon.Services.Category.Domain.IRepository;
using Axlon.Services.Category.Domain.IServices;
using Axlon.Services.Contracts.Category;
using Axlon.Services.Contracts.Category.Dto;
using Mapster;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using Polly;
using SqlSugar;

namespace Axlon.Services.Category.Infrastructure.Services
{
    public class CategoryDomainService:ICategoryDomainService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryDomainService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<List<long>> ByIdsGetChidrenIdsAsync(List<long> ids)
        {
            #region 递归
            /*var result = new HashSet<long>();

            var currentIds = ids;

            while (currentIds.Any())
            {
                var children = await categoryRepository.Db
                    .Queryable<Categories>()
                    .Where(x => currentIds.Contains(x.ParentId))
                    .Select(x => x.Id)
                    .ToListAsync();


                if (children.Count == 0)break;


                foreach (var id in children)
                {
                    result.Add(id);
                }


                currentIds = children;
            }

            return result.Distinct().ToList();*/
            #endregion

            var returnIds = ids;

            // 父节点路径 需要加'/'，来确认是否有下一个节点
            var queryPathStart = await categoryRepository.Query(x => x.Path + "/", x => ids.Contains(x.Id), "path");

            //满足条件的子节点路径
            var paths = await categoryRepository.Query(x => x.Path, x => queryPathStart.Any(q => x.Path.StartsWith(q)), "path");

            foreach (var path in paths) 
            {
                var startPath = queryPathStart.First(x => path.StartsWith(x));
                var endPath = path.Split(startPath)[1].Split("/");
                returnIds.AddRange(endPath.Select(x => long.Parse(x)));
            }

            return returnIds;
        }

        public async Task<List<CategoryNodeDto>> GetChildrenAsync(long id)
        {
            var res = await categoryRepository.Query(
                whereExpression: x => x.ParentId.Equals(id) && x.Status == 1, "sort");

            return res.Adapt<List<CategoryNodeDto>>();
        }

        public async Task<List<CategoryNodeDto>> GetTopNodeAsync()
        {
            var res = await categoryRepository.Query(
                whereExpression: x => x.Level == 1 && x.Status == 1,"sort");

            return res.Adapt<List<CategoryNodeDto>>();
        }

        public async Task<List<CategoryNodeDto>> GetTreeAsync()
        {
            var tree = await categoryRepository.Db.Queryable<Categories>()
                .OrderBy(x => x.Sort)
                .ToTreeAsync(x => x.Children, x => x.ParentId, 0, x => x.Id);

            return tree.Adapt<List<CategoryNodeDto>>();

        }
    }
}
