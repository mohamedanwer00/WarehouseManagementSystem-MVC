using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseBLL.BusinessServices.View_Models.SalesInvoice;

public class SalesInvoiceItemViewModel
{
    public string ProductName { get; set; } = null!;

    public string UnitName { get; set; } = null!;

    public decimal SellingPrice { get; set; }

    public decimal Quantity { get; set; }

    public decimal? Discount { get; set; }

    public decimal TotalPrice { get; set; }
}
