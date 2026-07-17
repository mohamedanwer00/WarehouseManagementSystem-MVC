using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WarehouseDAL.Entities.Enums;

namespace WarehouseBLL.FormViewModels.Supplier
{
    public class SupplierFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "اسم المورد مطلوب")]
        [StringLength(150, MinimumLength = 3,
            ErrorMessage = "اسم المورد يجب أن يكون بين 3 و150 حرف")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^01[0125][0-9]{8}$",
            ErrorMessage = "رقم الهاتف المصري غير صحيح")]
        [StringLength(11, MinimumLength = 11,
            ErrorMessage = "رقم الهاتف يجب أن يتكون من 11 رقم")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(250, MinimumLength = 5,
            ErrorMessage = "العنوان يجب أن يكون بين 5 و250 حرف")]
        public string Address { get; set; } = null!;

        [Range(0, 999999999.99,
            ErrorMessage = "الرصيد الافتتاحي يجب أن يكون أكبر من أو يساوي صفر")]
        public decimal OpeningBalance { get; set; }

        [Required(ErrorMessage = "يرجى اختيار طبيعة الحساب")]
        public BalanceType OpeningBalanceType { get; set; } = BalanceType.Creditor;

        public List<SelectListItem> BalanceTypes { get; set; } = new()
        {
            new SelectListItem
            {
                Value = ((int)BalanceType.Creditor).ToString(),
                Text = "دائن (له فلوس)"
            },
            new SelectListItem
            {
                Value = ((int)BalanceType.Debitor).ToString(),
                Text = "مدين (عليه فلوس)"
            }
        };
    }
}