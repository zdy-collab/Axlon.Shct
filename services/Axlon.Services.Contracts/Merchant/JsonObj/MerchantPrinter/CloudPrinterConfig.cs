namespace Axlon.Services.Contracts.Merchant.JsonObj.MerchantPrinter
{
    /// <summary>
    /// 云打印机连接配置
    /// </summary>
    public class CloudPrinterConfig
    {
        /// <summary>云打印机SN码</summary>
        public string Sn { get; set; }

        /// <summary>云打印机API Key</summary>
        public string ApiKey { get; set; }

        /// <summary>云打印机API Secret</summary>
        public string ApiSecret { get; set; }
    }
}
