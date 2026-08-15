namespace Axlon.Services.Contracts.Merchant.JsonObj.MerchantPrinter
{
    /// <summary>
    /// 蓝牙连接配置
    /// </summary>
    public class BluetoothConfig
    {
        /// <summary>蓝牙MAC地址</summary>
        public string MacAddress { get; set; }

        /// <summary>蓝牙设备名称</summary>
        public string DeviceName { get; set; }
    }
}
