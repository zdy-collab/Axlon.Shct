namespace Axlon.Services.Contracts.BdGeography.Dto
{
    /// <summary>
    /// 百度API - 附近定位推荐的poi_types参数
    /// 按优先级排序，用'|'分割
    /// </summary>
    public static class PoiTypesRecommendation
    {
        public const string ByLocationGetNamePoi = "商场|购物中心|住宅区|商务写字楼|公园|景点|酒店";

        /// <summary>
        /// 方案一：精简推荐（只召回最有价值的地标类型）
        /// 适用场景：空间有限，只展示5-10个结果
        /// </summary>
        public const string Compact =
            "房地产|购物|交通设施|旅游景点|教育|医疗|政府机构|酒店|运动健身";

        /// <summary>
        /// 方案二：标准推荐（美团风格）
        /// 适用场景：常规附近地点选择，展示10-20个结果
        /// </summary>
        public const string Standard = "商圈|购物中心|公园|景点|住宅区|写字楼|医院|学校|地铁站|火车站|公司企业";
        //"房地产|购物|交通设施|旅游景点|教育|医疗|政府机构|酒店|" +
        //"休闲娱乐|运动健身|文化传媒|公司企业|金融|自然地物|绿地";

        /// <summary>
        /// 方案三：全面推荐（返回所有有价值的类型）
        /// 适用场景：需要更多选择，展示20+个结果
        /// </summary>
        public const string Comprehensive =
            "房地产|购物|交通设施|旅游景点|教育|医疗|政府机构|酒店|" +
            "休闲娱乐|运动健身|文化传媒|公司企业|金融|自然地物|绿地|" +
            "出入口|行政地标|行政区划";

        /// <summary>
        /// 被排除的类型（无定位价值或干扰项）
        /// </summary>
        public const string Excluded =
            "美食|生活服务|丽人|汽车服务|电子眼|标注|" +
            "行政界线|其他线要素|门址|公交线路|道路|水系|铁路";
    }
}
