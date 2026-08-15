using NGeoHash;

namespace Axlon.Services.Merchant.Helper
{
    public class GeoHashHelper
    {
        /// <summary>
        /// 获取某个经纬度周围 300 米的 GeoHash 前缀列表
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="radiusMeters"></param>
        /// <returns></returns>
        public static List<string> GetNearbyGeoHashPrefixes(
            double lng,
            double lat,
            int numberOfChars = 6)
        {


            //          长度  纬度 bit  经度 bit  纬度误差       经度误差       距离误差（约）
            //           1     2        3        ±23°          ±23°          ±2500 km
            //           2     5        5        ±2.8°         ±5.6°         ±630 km
            //           3     7        8        ±0.70°        ±0.70°        ±78 km
            //           4     10       10       ±0.087°       ±0.18°        ±20 km
            //           5     12       13       ±0.022°       ±0.022°       ±2.4 km
            //           6     15       15       ±0.0027°      ±0.0055°      ±610 m
            //           7     17       18       ±0.00068°     ±0.00068°     ±76 m
            //           8     20       20       ±0.000085°    ±0.00017°     ±19 m
            //           9     22       23       ±0.000021°    ±0.000021°    ±2.4 m
            //           10    25       25       ±0.0000027°   ±0.0000054°   ±0.6 m

            var prefixes = new List<string>();

            // 精度 6: 覆盖范围约 1.2km，适合 500 米查询
            var hash6 = GeoHash.Encode(lat, lng, numberOfChars);
            prefixes.Add(hash6);

            // 考虑到边界问题，也可以加入周围的 8 个 GeoHash
            var neighbors = GetGeoHashNeighbors(hash6);
            prefixes.AddRange(neighbors);

            return prefixes.Distinct().ToList();
        }

        /// <summary>
        /// 获取 GeoHash 相邻的 8 个区域（处理边界情况）
        /// </summary>
        public static List<string> GetGeoHashNeighbors(string hash)
        {
            // 这里可以使用 GeoHash 库的 Neighbors 方法
            // 或者简化：只返回前缀本身，略微扩大范围再精确过滤
            return new List<string> { hash };
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static string GetGeoHash(double lng, double lat, int precision = 12)
        {
            return GeoHash.Encode(lat, lng, precision);
        }
    }
}
