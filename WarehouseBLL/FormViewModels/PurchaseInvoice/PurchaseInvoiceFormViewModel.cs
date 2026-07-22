using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WarehouseBLL.FormViewModels.PurchaseInvoice;

public class PurchaseInvoiceFormViewModel
{
    public int? Id { get; set; }

    public string? InvoiceNumber { get; set; }

    [Required(ErrorMessage = "تاريخ الفاتورة مطلوب")]
    public DateTime InvoiceDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "المورد مطلوب")]
    public int SupplierId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> Suppliers { get; set; } = Enumerable.Empty<SelectListItem>();

    [Required(ErrorMessage = "الفرع مطلوب")]
    public int BranchId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> Branches { get; set; } = Enumerable.Empty<SelectListItem>();

    [Required(ErrorMessage = "المخزن مطلوب")]
    public int WarehouseId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> Warehouses { get; set; } = Enumerable.Empty<SelectListItem>();

    public int? CashBoxId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> CashBoxes { get; set; } = Enumerable.Empty<SelectListItem>();

    public PaymentMethod PaymentMethod { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> PaymentMethods { get; set; } = Enumerable.Empty<SelectListItem>();

    public decimal? Discount { get; set; } = 0;

    public decimal TotalAmount { get; set; } = 0;

    public decimal? Paid { get; set; } = 0;

    public decimal? Remaining { get; set; } = 0;

    public string? Notes { get; set; }

    public List<PurchaseInvoiceItemFormViewModel> Items { get; set; } = new();
}
