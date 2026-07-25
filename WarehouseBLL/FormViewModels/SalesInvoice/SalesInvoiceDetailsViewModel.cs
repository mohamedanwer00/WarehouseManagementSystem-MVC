using WarehouseBLL.BusinessServices.View_Models.SalesInvoice;

namespace WarehouseBLL.FormViewModels.SalesInvoice;

public class SalesInvoiceDetailsViewModel
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public DateTime InvoiceDate { get; set; }

    public string CustomerName { get; set; } = null!;

    public string BranchName { get; set; } = null!;

    public string WarehouseName { get; set; } = null!;

    public decimal? Discount { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? Paid { get; set; }

    public decimal? Remaining { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public InvoiceStatus Status { get; set; }

    public string? Notes { get; set; }

    public IEnumerable<SalesInvoiceItemViewModel> Items { get; set; } = [];
}
