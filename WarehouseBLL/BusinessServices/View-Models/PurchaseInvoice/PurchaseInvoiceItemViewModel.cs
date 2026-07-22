namespace WarehouseBLL.BusinessServices.View_Models.PurchaseInvoice;

public class PurchaseInvoiceItemViewModel
{
    public string ProductName { get; set; } = null!;

    public string UnitName { get; set; } = null!;

    public decimal PurchasePrice { get; set; }

    public double Quantity { get; set; }

    public decimal? Discount { get; set; }

    public decimal TotalPrice { get; set; }
}

