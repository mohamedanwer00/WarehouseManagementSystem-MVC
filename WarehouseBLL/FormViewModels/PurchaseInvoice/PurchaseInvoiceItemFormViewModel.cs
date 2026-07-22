using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WarehouseBLL.FormViewModels.PurchaseInvoice;

public class PurchaseInvoiceItemFormViewModel
{
    [Required(ErrorMessage = "المنتج مطلوب")]
    [Range(1, int.MaxValue, ErrorMessage = "يرجى اختيار المنتج")]
    public int ProductId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> Products { get; set; } = Enumerable.Empty<SelectListItem>();

    [Required(ErrorMessage = "الوحدة مطلوبة")]
    [Range(1, int.MaxValue, ErrorMessage = "يرجى اختيار الوحدة")]
    public int ProductUnitId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> Units { get; set; } = Enumerable.Empty<SelectListItem>();

    [Range(0, double.MaxValue, ErrorMessage = "سعر الشراء يجب أن يكون 0 أو أكثر")]
    public decimal PurchasePrice { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من 0")]
    public decimal Quantity { get; set; } = 1;

    public decimal? Discount { get; set; } = 0;

    public decimal TotalPrice { get; set; }
}

