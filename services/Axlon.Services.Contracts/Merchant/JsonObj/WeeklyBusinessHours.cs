using System;
using System.Collections.Generic;
using System.Text;

namespace Axlon.Services.Contracts.Merchant.JsonObj
{
    public class WeeklyBusinessHours
    {
        public BusinessHours Monday { get; set; }
        public BusinessHours Tuesday { get; set; }
        public BusinessHours Wednesday { get; set; }
        public BusinessHours Thursday { get; set; }
        public BusinessHours Friday { get; set; }
        public BusinessHours Saturday { get; set; }
        public BusinessHours Sunday { get; set; }
    }

    public class BusinessHours
    {
        public string Open { get; set; }
        public string Close { get; set; }
    }
}
