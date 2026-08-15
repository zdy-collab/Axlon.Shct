namespace Axlon.Services.Contracts.Merchant.JsonObj.MerchantPrinter
{
    public class PrinterConnectionConfig
    {
        public List<BluetoothConfig> BluetoothConfigs { get; set; }

        public List<CloudPrinterConfig> CloudPrinterConfigs { get; set; }
    }
}
