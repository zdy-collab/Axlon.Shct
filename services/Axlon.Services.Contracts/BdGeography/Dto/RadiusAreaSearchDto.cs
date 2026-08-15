using System.Text.Json.Serialization;

namespace Axlon.Services.Contracts.BdGeography.Dto
{
    #region req
    public record RadiusAreaSearchReq(string query, double longitude, double latitude, long radius = 5000);

    #endregion

    #region res
    public class RadiusAreaSearchRes
    {
        public int status { get; set; }

        public string message { get; set; }

        public int total { get; set; }

        public string result_type { get; set; }

        public List<RAS_Result> results { get; set; }
    }


    public class RAS_Result
    {
        public string name { get; set; }

        public BaiduLocation location { get; set; }

        public string address { get; set; }

        public string province { get; set; }

        public string city { get; set; }

        public string area { get; set; }

        public int adcode { get; set; }

        public string telephone { get; set; }

        public string uid { get; set; }

        public string status { get; set; }

        public string street_id { get; set; }

        [JsonIgnore]
        public string detail { get; set; }

        public RAS_DetailInfo detail_info { get; set; }

        public List<RAS_ChildPoi> children { get; set; }
    }


    public class RAS_DetailInfo
    {
        public string classified_poi_tag { get; set; }

        public int distance { get; set; }

        public string type { get; set; }

        public string tag { get; set; }

        public string label { get; set; }


        public BaiduLocation navi_location { get; set; }


        public string price { get; set; }

        public string shop_hours { get; set; }


        public string overall_rating { get; set; }

        public string taste_rating { get; set; }

        public string service_rating { get; set; }

        public string environment_rating { get; set; }

        public string facility_rating { get; set; }

        public string hygiene_rating { get; set; }

        public string technology_rating { get; set; }


        public string image_num { get; set; }

        public int groupon_num { get; set; }

        public int discount_num { get; set; }

        public string comment_num { get; set; }

        public string favorite_num { get; set; }

        public string checkin_num { get; set; }


        public string brand { get; set; }

        public string content_tag { get; set; }


        public List<string> photos { get; set; }
    }


    public class BaiduNaviLocation
    {
        public double lng { get; set; }

        public double lat { get; set; }
    }


    public class RAS_ChildPoi
    {
        public string uid { get; set; }

        public string name { get; set; }

        public string show_name { get; set; }

        public string tag { get; set; }

        public BaiduLocation location { get; set; }

        public string address { get; set; }
    }
    #endregion
}
