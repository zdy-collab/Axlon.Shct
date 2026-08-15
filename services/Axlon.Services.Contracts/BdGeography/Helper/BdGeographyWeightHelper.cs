using Axlon.Services.Contracts.BdGeography.Dto;

namespace Axlon.Services.Contracts.BdGeography.Helper
{
    /// <summary>
    /// 暂时分开类写
    /// </summary>
    public static class BdGeographyWeightHelper
    {


        /// <summary>
        /// POI分类权重
        /// </summary>
        private static readonly Dictionary<string, double> CategoryWeights =
            new()
            {

            // 商圈
            {"行政地标-商圈",1.00},
            {"行政地标-热点区域",0.90},


            // 商业
            {"购物-购物中心",0.95},
            {"购物-百货商场",0.90},
            {"购物-市场",0.60},


            // 交通
            {"交通设施-飞机场",1.00},
            {"交通设施-火车站",1.00},
            {"交通设施-地铁站",0.90},
            {"交通设施-长途汽车站",0.75},


            // 景区
            {"旅游景点-风景区",0.95},
            {"旅游景点-公园",0.90},
            {"旅游景点-景点",0.85},


            // 办公
            {"房地产-写字楼",0.75},
            {"公司企业-园区",0.65},


            // 住宅
            {"房地产-住宅区",0.45},


            // 教育医疗
            {"教育-高等院校",0.70},
            {"医疗-综合医院",0.70},


            // 酒店
            {"酒店-星级酒店",0.65},


            // 娱乐
            {"休闲娱乐-电影院",0.45},
            {"运动健身-体育场馆",0.50}

            };




        /// <summary>
        /// 无效关键词
        /// </summary>
        private static readonly string[] InvalidKeywords =
        {
        "停车场",
        "地下",
        "地上",

        "管理用房",
        "值班室",
        "办公室",
        "服务中心",
        "游客中心",
        "售票处",

        "入口",
        "出口",

        "楼栋",
        "栋"
    };



        /// <summary>
        /// 商户过滤
        /// 地址选择不展示普通商户
        /// </summary>
        private static readonly string[] MerchantKeywords =
        {
        "便利店",
        "超市",
        "副食",
        "粮油",
        "烟酒",
        "批发",
        "五金",
        "建材",

        "餐厅",
        "饭店",
        "火锅",
        "烧烤",
        "小吃",

        "美容",
        "美发",
        "维修",
        "洗车"
    };




        private static bool IsInvalid(string name)
        {

            if (string.IsNullOrWhiteSpace(name))
                return true;


            return InvalidKeywords.Any(x =>
                name.Contains(x));

        }



        private static bool IsMerchant(string name)
        {

            if (string.IsNullOrWhiteSpace(name))
                return false;


            return MerchantKeywords.Any(x =>
                name.Contains(x));

        }





        /// <summary>
        /// 名称增强
        /// </summary>
        private static double NameBoost(string name)
        {

            if (string.IsNullOrWhiteSpace(name))
                return 0;



            string[] landmark =
            {
            "万达",
            "万象城",
            "广场",
            "中心",
            "公园",
            "景区",
            "车站",
            "机场"
        };


            if (landmark.Any(x => name.Contains(x)))
                return 0.15;



            string[] building =
            {
            "大厦",
            "大楼",
            "中心"
        };


            if (building.Any(x => name.Contains(x)))
                return 0.08;



            return ResidentialScore(name);

        }





        /// <summary>
        /// 住宅名称评分
        /// </summary>
        private static double ResidentialScore(string name)
        {

            if (name.Contains("家园"))
                return 0.08;


            if (name.Contains("花园"))
                return 0.06;


            if (name.Contains("苑"))
                return 0.05;



            if (name.Contains("小区"))
            {

                // 碧津公园正街小区
                if (name.Length >= 8)
                    return 0.05;


                return 0.02;
            }



            // 社区不是用户常用地址
            if (name.Contains("社区"))
                return -0.05;



            return 0;

        }





        private static double GetCategoryWeight(string tag)
        {

            if (string.IsNullOrWhiteSpace(tag))
                return 0.2;



            foreach (var item in CategoryWeights)
            {

                if (tag.Contains(item.Key))
                    return item.Value;

            }


            return 0.25;

        }




        /// <summary>
        /// 距离评分
        /// </summary>
        private static double DistanceScore(double distance)
        {

            if (distance <= 100)
                return 1;


            if (distance <= 300)
                return 0.95;


            if (distance <= 500)
                return 0.85;


            if (distance <= 1000)
                return 0.70;


            if (distance <= 2000)
                return 0.50;


            return 0.30;

        }





        /// <summary>
        /// 类型分组
        /// </summary>
        private static string GetGroup(
            string tag,
            string name)
        {


            if (tag.Contains("旅游景点"))
                return "景区";


            if (tag.Contains("购物"))
                return "商业";



            if (
                tag.Contains("写字楼")
                ||
                name.Contains("大厦")
                ||
                name.Contains("大楼"))
                return "办公";



            if (
                tag.Contains("房地产")
                ||
                name.Contains("小区")
                ||
                name.Contains("家园")
                ||
                name.Contains("花园")
                ||
                name.Contains("苑"))
                return "住宅";



            if (tag.Contains("酒店"))
                return "酒店";


            return "其他";

        }





        /// <summary>
        /// 去重
        /// </summary>
        private static List<GeocodingPoi> RemoveDuplicate(
            List<GeocodingPoi> list)
        {

            var result =
                new List<GeocodingPoi>();


            foreach (var item in list)
            {

                bool exists =
                    result.Any(x =>
                        x.name.Contains(item.name)
                        ||
                        item.name.Contains(x.name));


                if (!exists)
                    result.Add(item);

            }


            return result;

        }





        /// <summary>
        /// 最终排序
        /// </summary>
        public static List<GeocodingPoi> SortByComprehensiveWeight(
            List<GeocodingPoi> pois)
        {


            pois =
                pois
                .Where(x => !IsInvalid(x.name))
                .Where(x => !IsMerchant(x.name))
                .ToList();



            pois =
                RemoveDuplicate(pois);




            var rankings =
                pois
                .Select(p =>
                {


                    double category =
                        GetCategoryWeight(p.tag);



                    double distance =
                        DistanceScore(
                            double.TryParse(
                                p.distance,
                                out var d)
                                ? d
                                : 1000);



                    double score =
                        category * 0.35
                        +
                        distance * 0.45
                        +
                        NameBoost(p.name) * 0.20;



                    return new
                    {
                        Poi = p,
                        Score = score,
                        Group = GetGroup(
                            p.tag,
                            p.name)
                    };


                })
                .OrderByDescending(x => x.Score)
                .ToList();





            // 类型配额
            var limits =
                new Dictionary<string, int>
                {
                {"景区",2},
                {"商业",2},
                {"办公",2},
                {"住宅",3},
                {"酒店",1},
                {"其他",1}
                };



            var result =
                new List<GeocodingPoi>();



            foreach (var limit in limits)
            {

                result.AddRange(
                    rankings
                    .Where(x => x.Group == limit.Key)
                    .Take(limit.Value)
                    .Select(x => x.Poi)
                );

            }



            // 不足补充
            if (result.Count < 10)
            {

                result.AddRange(
                    rankings
                    .Where(x => !result.Contains(x.Poi))
                    .Take(10 - result.Count)
                    .Select(x => x.Poi)
                );

            }


            return result
                .GroupBy(x => x.name)
                .Select(x => x.First())
                .Take(10)
                .ToList();

        }

    }
}
