using Axlon.Services.Contracts.Merchant.Enums;
using Axlon.Services.Contracts.Merchant.JsonObj.MerchantPrinter;
using Axlon.Services.Contracts.Merchant.RootTkey;
using SqlSugar;

namespace Axlon.Services.Contracts.Merchant
{
    /// <summary>
    /// 打印机配置表
    /// </summary>
    [Tenant("Main")]
    [SugarTable("merchant_printers", "打印机配置表")]
    public class MerchantPrinter : MerchantPrintersRoot<long>
    {

        /// <summary>
        /// 打印机类型：bluetooth-蓝牙 / cloud-云打印机
        /// </summary>
        [SugarColumn(ColumnName = "printer_type", ColumnDataType = "varchar(20)", IsNullable = false)]
        public string PrinterType { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        [SugarColumn(ColumnName = "printer_name", ColumnDataType = "varchar(100)", IsNullable = false)]
        public string PrinterName { get; set; }

        /// <summary>
        /// 连接配置（JSON格式存储，蓝牙MAC地址/云打印机SN）
        /// </summary>
        [SugarColumn(ColumnName = "connection_config", ColumnDataType = "json", IsJson = true, IsNullable = false)]
        public PrinterConnectionConfig ConnectionConfig { get; set; }

        /// <summary>
        /// 打印菜品分类（JSON数组，如["凉菜","热菜"]）
        /// </summary>
        [SugarColumn(ColumnName = "print_categories", ColumnDataType = "json", IsJson = true, IsNullable = true)]
        public List<string> PrintCategories { get; set; }

        /// <summary>
        /// 状态：1-启用 / 0-停用
        /// </summary>
        [SugarColumn(ColumnName = "status", ColumnDataType = "tinyint", IsNullable = false, DefaultValue = "1")]
        public PrinterStatus Status { get; set; } = PrinterStatus.启用;
    }
}
