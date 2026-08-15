using Axlon.Framework.Abstractions;
using Axlon.Framework.Core.Extensions;
using Axlon.Framework.Core.HttpContextUser;
using Axlon.Framework.Data.Services.Base;
using Axlon.Framework.EventBus;
using Axlon.Services.Contracts.Base;
using Axlon.Services.Contracts.Events;
using Axlon.Services.Contracts.Extensions;
using Axlon.Services.Contracts.GroupBuy.Enums;
using Axlon.Services.Contracts.Merchant;
using Axlon.Services.Contracts.Merchant.Dto;
using Axlon.Services.Contracts.Merchant.Enums;
using Axlon.Services.Merchant.Helper;
using Axlon.Services.Merchant.Repository;
using Axlon.Services.Merchant.ServiceInvocation.Category;
using Axlon.Services.Merchant.Services.Interfaces;
using CoordinateSharp;
using Mapster;
using SqlSugar;

namespace Axlon.Services.Merchant.Services
{
    public class MerchantServices : BaseServices<Merchants>, IMerchantServices
    {

        private readonly IMerchantsRepository merchantsRepository;
        //private readonly HttpClient categoryClient;
        private readonly ICategoryClient categoryClient;
        private readonly IUser user;
        private readonly IAxlonEventPublisher eventPublisher;
        private readonly ILogger<MerchantServices> logger;
        //private readonly 

        public MerchantServices(
            IMerchantsRepository merchantsRepository,
            ICategoryClient categoryClient,
            IUser user,
            IAxlonEventPublisher eventPublisher,
            ILogger<MerchantServices> logger) : base(merchantsRepository)
        {
            this.merchantsRepository = merchantsRepository;
            this.categoryClient = categoryClient;
            this.user = user;
            this.eventPublisher = eventPublisher;
            this.logger = logger;
            //this.categoryClient = factory.CreateClient(ServiceName.category.ToString());
        }

        public Task<MerchantsDto> ByIdGetMerchantsAsync(ByIdGetMerchantsReq req)
        {
            var dto = new MerchantsDto();

            var merchantsDto = merchantsRepository.QueryById(req.id).Adapt<MerchantsDto>();


            throw new NotImplementedException();
        }

        public async Task<PageResponseModel<ByJwGetMerchantListRes>> ByJwGetMerchantListAsync(ByJwGetMerchantListReq req)
        {
            PageResponseModel<ByJwGetMerchantListRes> res = new PageResponseModel<ByJwGetMerchantListRes>();

            // 起始点
            var user = new Coordinate((double)req.Latitude, (double)req.Longitude);

            //// 测试数据
            //req = new ByJwGetMerchantListReq()
            //{
            //    Longitude = 106.5047550m,
            //    Latitude = 29.6149550m
            //};

            var geo1 = GeoHashHelper.GetGeoHash(106.6318900, 29.7192500);  //得到当前点geo前缀坐标
            var geo2 = GeoHashHelper.GetGeoHash(106.6289700, 29.7167100);  //得到当前点geo前缀坐标
            //var geo3 = GeoHashHelper.GetGeoHash(116.3970290, 39.9177320);  //得到当前 点geo前缀坐标

            var geo = GeoHashHelper.GetNearbyGeoHashPrefixes((double)req.Longitude, (double)req.Latitude);  //得到当前点geo前缀坐标



            // 根据geo前缀查询商家
            var allMerchants = await merchantsRepository.Query(
                whereExpression: x => geo.Any(g => x.GeoHash.StartsWith(g))
            );

            // 计算两点距离
            var sortedList = allMerchants
                .Select(x => new
                {
                    Merchant = x,
                    Meter = (int)user.Get_Distance_From_Coordinate(
                        new Coordinate((double)x.Latitude, (double)x.Longitude)
                    ).Meters
                })
                .OrderBy(x => x.Meter)
                .ToList();

            // 内存分页
            var pagedList = sortedList
                .Skip((req.page - 1) * req.pageSize)
                .Take(req.pageSize)
                .ToList();

            // 映射Dto
            res.data = pagedList.Select(x => new ByJwGetMerchantListRes
            {
                merchants = x.Merchant.Adapt<MerchantsDto>(),
                meter = x.Meter
            }).ToList();

            res.dataCount = sortedList.Count;
            res.pageSize = req.pageSize;    // 回显
            res.page = req.page;    // 回显

            return res;
        }

        public async Task<QueryMerchantsRes> QueryMerchantsAsync(QueryMerchantsReq req)
        {
            // 起始点
            var startPoint = new Coordinate(req.location.Latitude, req.location.Longitude);

            // 获取附近GeoHash
            var geoHashes = new List<string>();

            foreach (var len in new[] { 6, 5, 4 })
            {
                foreach (var geo in GeoHashHelper.GetNearbyGeoHashPrefixes(req.location.Longitude, req.location.Latitude, len))
                {
                    geoHashes.Add(geo);
                }
            }

            // 准备返回的商家信息
            var merchantsMeterDto = new List<MerchantsMeterDto>();
            var returnMerchantsIds = new List<long>();

            // 距离优先
            if (req.sortBy == SortType.distance)
            {
                // 品类子Id
                var categoryIds = await categoryClient.ByIdsGetChidrenIdsAsync(req.categoryIds);

                var query = merchantsRepository.Db.Queryable<Merchants>()
                    .Where(x => !req.merchantsIds.Contains(x.Id));


                if (geoHashes != null && geoHashes.Any())
                {
                    query = query.Where(x => geoHashes.Any(g => SqlFunc.StartsWith(x.GeoHash, g)));
                }


                if (categoryIds != null && categoryIds.Any())
                {
                    query = query.Where(x =>
                        SqlFunc.Subqueryable<MerchantCategoryConfig>()
                        .Where(y =>
                            y.MerchantId == x.Id &&
                            categoryIds.Contains(y.CategoryId))
                        .Any());
                }


                var merchants = await query.ToListAsync();

                var merchatsIds = merchantsRepository.ByCategoryIdsGetMerchantIdsAsync(categoryIds);

                var expression = merchants.Select(x =>
                {
                    var meter =
                    startPoint.Get_Distance_From_Coordinate(
                        new Coordinate(
                            (double)x.Latitude,
                            (double)x.Longitude));

                    return new MerchantsMeterDto
                    {
                        merchants = x.Adapt<MerchantsDto>(),
                        meter = (int)meter.Meters
                    };
                })
                .WhereIf(req.mater != 0, x => x.meter <= req.mater)
                .OrderBy(x => x.meter);

                if (req.sortBy == SortType.distance) expression = expression.OrderBy(x => x.meter);

                var result = expression.Take(req.dataCount).ToList();

                var merchantsIds = result.Select(x => x.merchants.Id).ToList();

                return new QueryMerchantsRes
                {
                    merchantsMeters = result
                };
            }
            // 评分优先
            else if (req.sortBy == SortType.score)
            {
                // 筛选附近的商家
                var merchants = await merchantsRepository.Query(whereExpression: x => geoHashes.Any(g => g.StartsWith(x.GeoHash)));
                throw new Exception("评分优先未实现！");
            }
            // 销量优先
            else
            {
                throw new Exception("销量优先未实现！");
            }
        }

        public async Task<List<MerchantInfoDto>> NearbyMerchantQueryAsync(NearbyMerchantQueryReq req)
        {
            // 起始点
            var startPoint = new Coordinate(req.Latitude, req.Longitude);

            // 获取附近GeoHash,+ - 610m
            var geoHashes = GeoHashHelper.GetNearbyGeoHashPrefixes(req.Longitude, req.Latitude, 5);

            var merchants = await base.Db.Queryable<Merchants>()
                .Includes(x => x.merchantCategoryConfigs)
                .Where(x => x.Status == MerchantStatus.已通过)
                .WhereIF(StaticStatus.MerchantGoHashStatus,x => geoHashes.Any(g => x.GeoHash.StartsWith(g)))
                .ToListAsync();


            var _merchants = merchants.Select(x =>
            {
                var meter = new Random().Next(50, 600);

                if (StaticStatus.MerchantGoHashStatus)
                {
                    meter =
                    (int)startPoint.Get_Distance_From_Coordinate(
                        new Coordinate(
                            (double)x.Latitude,
                            (double)x.Longitude)).Meters;
                }

                return new
                {
                    Id = x.Id,
                    LogoFileId = x.LogoFileId,
                    Name = x.Name,
                    LogoOss = x.LogoOss,
                    //Logo = x.Logo,
                    Meter = meter,  // 距离
                    //PerCapita = new Random().Next(0, 101),  // 随机生成人均消费
                    RecommendNumber = new Random().Next(50, 2000),   // 随机生成推荐人数
                    //Score = Math.Round(4 + (double)new Random().NextDouble() * 1, 1),    // 随机生成评分
                    CategoryIds = x.merchantCategoryConfigs.Select(y => y.CategoryId).ToList()
                };
            })
            .OrderBy(x => x.Meter)
            //.Where(x => x.Meter <= 300)
            .ToList();

            // 3. 去重得到 categoryIds（处理字符串拆分）
            var categoryIds = _merchants
                .SelectMany(x => x.CategoryIds)
                .Distinct()
                .ToList();

            var categories = await categoryClient.ByIdsGetCategoriesAsync(categoryIds);

            var returnData = _merchants.Select(x => new MerchantInfoDto
            {
                Id = x.Id,
                Name = x.Name,
                LogoFileId = x.LogoFileId,
                LogoOss = x.LogoOss,
                //Logo = x.Logo,
                Meter = x.Meter,  // 距离
                Introduce = "实惠的" + x.Name,
                //PerCapita = x.PerCapita,
                RecommendNumber = x.RecommendNumber,
                //Score = x.Score,
                Categories = categories.Where(y => x.CategoryIds.Contains(y.Id)).ToList()
            }).ToList();

            return returnData;
        }

        public async Task<List<MerchantInfoDto>> RecommendMerchantQueryAsync(RecommendMerchantQueryReq req)
        {
            // 起始点
            var startPoint = new Coordinate(req.Latitude, req.Longitude);

            // 获取附近GeoHash,+ - 610m
            var geoHashes = GeoHashHelper.GetNearbyGeoHashPrefixes(req.Longitude, req.Latitude, 5);

            var merchants = await merchantsRepository.Db
            .Queryable<Merchants>()
            .Where(x => x.Status == MerchantStatus.已通过)
            .WhereIF(StaticStatus.MerchantGoHashStatus,x => geoHashes.Any(g => x.GeoHash.StartsWith(g)))
            .Select(x => new
            {
                x.Id,
                x.LogoFileId,
                x.Name,
                x.LogoOss,
                Introduce = "实惠的" + x.Name,
                x.GeoHash,
                //x.Logo,
                x.Longitude,
                x.Latitude
            })
            .ToListAsync();


            var _merchants = merchants.Select(x =>
            {
                var meter = new Random().Next(50, 600);

                if (StaticStatus.MerchantGoHashStatus)
                {
                    meter =
                    (int)startPoint.Get_Distance_From_Coordinate(
                        new Coordinate(
                            (double)x.Latitude,
                            (double)x.Longitude)).Meters;
                }

                return new MerchantInfoDto
                {
                    Id = x.Id,
                    LogoFileId = x.LogoFileId,
                    Name = x.Name,
                    LogoOss = x.LogoOss,
                    //Logo = x.Logo,
                    Introduce = x.Introduce,
                    Meter = meter,  // 距离
                    //Meter = new Random().Next(50, 600),  // 随机生成距离
                    PerCapita = new Random().Next(0, 101),  // 随机生成人均消费
                    //RecommendNumber = new Random().Next(50, 2000),   // 随机生成推荐人数
                    Score = Math.Round(4 + (double)new Random().NextDouble() * 1, 1)    // 随机生成评分
                    //CategoryIds = x.CategoryIds.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(y => long.Parse(y)).ToList()
                };
            })
            .OrderByDescending(x => x.Score)
            .ToList();

            return _merchants;
        }

        public async Task<List<MerchantInfoDto>> SearchMerchantQueryAsync(SearchMerchantQueryReq req)
        {
            var tag = new List<string> { "明码标价", "无隐形消费", "达人推荐", "物超所值" };
            // 起始点
            var startPoint = new Coordinate(req.Latitude, req.Longitude);

            // 获取附近GeoHash
            var geoHashes = new List<string>();

            foreach (var len in new[] { 6, 5, 4 })
            {
                foreach (var geo in GeoHashHelper.GetNearbyGeoHashPrefixes(req.Longitude, req.Latitude, len))
                {
                    geoHashes.Add(geo);
                }
            }

            // 准备返回的商家信息
            //var merchantsMeterDto = new List<MerchantInfoDto>();
            //var returnMerchantsIds = new List<long>();

            //// 距离优先
            //if (req.sortBy == SortType.Distance)
            //{

            var query = merchantsRepository.Db.Queryable<Merchants>()
                .Where(x => !req.MerchantsIds.Contains(x.Id))
                .Where(x => x.Status == MerchantStatus.已通过)
                .WhereIF(StaticStatus.MerchantGoHashStatus,x => geoHashes.Any(g => SqlFunc.StartsWith(x.GeoHash, g)));

            if (req.CategoryIds != null && req.CategoryIds.Any())
            {
                // 品类子Id
                var categoryIds = await categoryClient.ByIdsGetChidrenIdsAsync(req.CategoryIds);
                if (categoryIds != null && categoryIds.Any())
                {
                    query = query.Where(x =>
                        SqlFunc.Subqueryable<MerchantCategoryConfig>().Where(y =>
                            y.MerchantId == x.Id &&
                            categoryIds.Contains(y.CategoryId))
                        .Any());
                }
            }

            var merchants = await query.ToListAsync();

            var expression = merchants.Select(x => {
                var meter = new Random().Next(50, 600);

                if (StaticStatus.MerchantGoHashStatus)
                {
                    meter =
                    (int)startPoint.Get_Distance_From_Coordinate(
                        new Coordinate(
                            (double)x.Latitude,
                            (double)x.Longitude)).Meters;
                }
                return new MerchantInfoDto
                {
                    Id = x.Id,
                    LogoFileId = x.LogoFileId,
                    Name = x.Name,
                    LogoOss = x.LogoOss,
                    //Logo = x.Logo,
                    Tags = tag.Skip(new Random().Next(0, 3)).Take(1).ToList(),
                    Introduce = "实惠的" + x.Name,
                    Meter = meter,
                    Score = Math.Round(4 + (double)new Random().NextDouble() * 1, 1),    // 随机生成评分
                    SalesVolume = new Random().Next(50, 2000),   // 随机生成销量

                };
            })
        .WhereIf(req.Mater != 0, x => x.Meter <= req.Mater);

            if (req.SortBy == SortType.distance) expression = expression.OrderBy(x => x.Meter);

            else if (req.SortBy == SortType.score) expression = expression.OrderByDescending(x => x.Score);

            else if (req.SortBy == SortType.salesVolume) expression = expression.OrderByDescending(x => x.SalesVolume);

            //var result = expression.Take(req.dataCount).ToList();

            //var merchantsIds = result.Select(x => x.merchants.Id).ToList();
            var PageResponseModel = new PageResponseModel<MerchantInfoDto>();

            return expression.ToList();
            //}
            //// 评分优先
            //else if (req.sortBy == SortType.Score)
            //{
            //    // 筛选附近的商家
            //    var merchants = await merchantsRepository.Query(whereExpression: x => geoHashes.Any(g => g.StartsWith(x.GeoHash)));
            //    throw new Exception("评分优先未实现！");
            //}
            //// 销量优先
            //else
            //{
            //    throw new Exception("销量优先未实现！");
            //}
        }

        public async Task<MerchantInfoDto> MiniGetMerchantDetailsAsync(MiniGetMerchantDetailsReq req)
        {
            // 起始点
            var startPoint = new Coordinate(req.Latitude, req.Longitude);

            var merchant = await base.Db.Queryable<Merchants>()
                .Includes(x => x.groupBuys.Where(y => y.IsOn == IsOn.上架
                && y.StartTime <= DateTime.Now && y.EndTime >= DateTime.Now).ToList())
                .Includes(x => x.productCategories.OrderByDescending(y => y.Sort).ToList(), y => y.products.OrderByDescending(z => z.Sort).ToList())
                .FirstAsync(x => x.Id == req.MerchantId);

            var dto = merchant.Adapt<MerchantInfoDto>();

            dto.Meter = (int)startPoint.Get_Distance_From_Coordinate(
                        new Coordinate(
                            (double)merchant.Latitude,
                            (double)merchant.Longitude)).Meters;

            //dto.PerCapita = new Random().Next(0, 101);  // 随机生成人均消费

            dto.Score = Math.Round(4 + (double)new Random().NextDouble() * 1, 1);    // 随机生成评分

            dto.Tags = new List<string>() { "明码标价", "无隐形消费" };

            //dto.groupBuys.ForEach(x => x.Image = x.Image.CombinFileAccessPath());

            // 差菜品

            await TryPublishMerchantViewAsync(dto);

            return dto;
        }

        private async Task TryPublishMerchantViewAsync(MerchantInfoDto merchant)
        {
            if (user.ID <= 0 || user.GetIsVisitor()) return;

            try
            {
                await eventPublisher.PublishAsync(
                    UserFootprintTopics.ViewedV1,
                    new UserFootprintViewedIntegrationEvent(
                        user.ID,
                        UserFootprintTargetTypes.Merchant,
                        merchant.Id,
                        merchant.Id,
                        TargetTitle: merchant.Name,
                        TargetImage: merchant.Logo));
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception,
                    "发布商铺浏览足迹失败，UserId={UserId}, MerchantId={MerchantId}", user.ID, merchant.Id);
            }
        }

        public async Task<MerchantBasic_TableDto> GetMerchantBasicAsync(long merchantId, List<long> tableIds)
        {
            var merchant = await base.Db.Queryable<Merchants>()
                .Includes(x => x.merchantTables.Where(y => tableIds.Contains(y.Id)).ToList())
                .Where(x => x.Id == merchantId)
                .FirstAsync();

            var res = merchant.Adapt<MerchantBasic_TableDto>();
            return res;
        }
    }
}
