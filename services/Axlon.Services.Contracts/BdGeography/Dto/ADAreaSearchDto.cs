namespace Axlon.Services.Contracts.BdGeography.Dto
{
    #region res

    /// <summary>
    /// 行政区域检索响应
    /// </summary>
    public class ADAreaSearchRes
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 结果类型
        /// </summary>
        public string result_type { get; set; }

        /// <summary>
        /// 查询类型
        /// </summary>
        public string query_type { get; set; }

        /// <summary>
        /// POI列表
        /// </summary>
        public List<ADAS_Results> results { get; set; }
    }

    /// <summary>
    /// POI地址信息
    /// </summary>
    public class ADAS_Results
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }


        /// <summary>
        /// 坐标
        /// </summary>
        public BaiduLocation location { get; set; }


        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }


        /// <summary>
        /// 省
        /// </summary>
        public string province { get; set; }


        /// <summary>
        /// 市
        /// </summary>
        public string city { get; set; }


        /// <summary>
        /// 区
        /// </summary>
        public string area { get; set; }


        /// <summary>
        /// 街道ID
        /// </summary>
        public string street_id { get; set; }


        /// <summary>
        /// 是否详细POI
        /// </summary>
        public int detail { get; set; }


        /// <summary>
        /// 百度唯一ID
        /// </summary>
        public string uid { get; set; }
    }
    #endregion
}
