namespace Axlon.Services.Contracts.BdGeography.Helper
{
    /// <summary>
    /// 百度逆地理编码POI筛选帮助类
    /// 用于小程序首页定位地标展示
    /// </summary>
    public static class BaiduGeographyHelper
    {
        /// <summary>
        /// 首页定位选择
        /// 
        /// 规则:
        /// 1. 优先知名地标
        /// 2. 其次热度
        /// 3. 最后距离
        /// 
        /// 用于:
        /// 小程序首页左上角定位展示
        /// </summary>
        public static BaiduPoiInfo? SelectForHomePage(
            IEnumerable<BaiduPoiInfo>? pois)
        {
            if (pois == null)
                return null;


            var excludeWords = new[]
            {
                "停车场",
                "停车",
                "道路",
                "路口",
                "公交",
                "车站",
                "收费站",
                "加油站",
                "厕所",
                "卫生间",
                "入口",
                "出口",
                "楼栋",
                "单元"
            };


            var typeWeights = new Dictionary<string, int>
        {
            { "购物中心", 130 },
            { "商场",130 },
            { "购物",120 },

            { "旅游景点",120 },
            { "景点",120 },
            { "公园",120 },

            { "商务写字楼",100 },
            { "写字楼",100 },
            { "大厦",90 },

            { "住宅区",90 },
            { "小区",90 },

            { "酒店",80 },

            { "医院",60 },
            { "学校",60 }
        };


            return pois
                .Where(p =>
                {
                    if (string.IsNullOrWhiteSpace(p.name))
                        return false;


                    var text =
                        $"{p.name};{p.tag};{p.poiType}";


                    return !excludeWords.Any(x =>
                        text.Contains(x,
                        StringComparison.OrdinalIgnoreCase));

                })
                .Select(p =>
                {
                    var text =
                        $"{p.poiType};{p.tag}";


                    // 类型分
                    var typeScore = 20;


                    foreach (var item in typeWeights)
                    {
                        if (text.Contains(
                            item.Key,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            typeScore = item.Value;
                            break;
                        }
                    }


                    // 热度分
                    var popularityScore = 0;


                    if (int.TryParse(
                        p.popularity_level,
                        out var level))
                    {
                        popularityScore = level switch
                        {
                            9 => 50,
                            8 => 45,
                            7 => 40,
                            6 => 35,
                            5 => 30,
                            4 => 25,
                            3 => 20,
                            2 => 10,
                            1 => 5,
                            _ => 0
                        };
                    }


                    // 距离分
                    var distanceScore = 0d;


                    if (int.TryParse(
                        p.distance,
                        out var distance))
                    {
                        distanceScore =
                            distance / 20d;
                    }


                    /*
                     * 首页定位评分:
                     *
                     * 类型 > 热度 > 距离
                     *
                     */
                    p.score =
                        typeScore * 10
                        + popularityScore
                        - distanceScore;


                    return p;

                })
                .OrderByDescending(x => x.score)
                .FirstOrDefault();
        }
    }



    /// <summary>
    /// 百度POI对象
    /// </summary>
    public abstract class BaiduPoiInfo
    {

        /// <summary>
        /// POI名称
        /// </summary>
        public abstract string name { get; set; }


        /// <summary>
        /// 地址
        /// </summary>
        public abstract string addr { get; set; }


        /// <summary>
        /// POI一级类型
        /// </summary>
        public abstract string poiType { get; set; }


        /// <summary>
        /// 标签
        /// 示例:
        /// 房地产;住宅区
        /// 旅游景点;公园
        /// </summary>
        public abstract string tag { get; set; }



        /// <summary>
        /// 距离(米)
        /// 百度返回string
        /// </summary>
        public abstract string distance { get; set; }



        /// <summary>
        /// 热度等级
        /// 1-9
        /// 9最高
        /// </summary>
        public abstract string popularity_level { get; set; }



        /// <summary>
        /// 内部计算评分
        /// </summary>
        public double score { get; set; }

    }
}
