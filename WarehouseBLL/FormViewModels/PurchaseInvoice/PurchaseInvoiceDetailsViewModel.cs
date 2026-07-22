using WarehouseBLL.BusinessServices.View_Models.PurchaseInvoice;

namespace WarehouseBLL.FormViewModels.PurchaseInvoice;

public class PurchaseInvoiceDetailsViewModel
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public DateTime InvoiceDate { get; set; }

    public string SupplierName { get; set; } = null!;

    public string BranchName { get; set; } = null!;

    public string WarehouseName { get; set; } = null!;

    public decimal? Discount { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? Paid { get; set; }

    public decimal? Remaining { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public InvoiceStatus Status { get; set; }

    public string? Notes { get; set; }

    public IEnumerable<PurchaseInvoiceItemViewModel> Items { get; set; } = [];
}

