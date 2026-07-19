using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseDAL.Entities.Enums;

namespace WarehouseBLL.FormViewModels.Customer
{
    public class CustomerFormViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public decimal OpeningBalance { get; set; }
        public BalanceType OpeningBalanceType { get; set; }
        public CustomerType CustomerType { get; set; }

        public List<SelectListItem> BalanceTypes { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = ((int)BalanceType.Debitor).ToString(), Text = "مدين (عليه فلوس)" },
            new SelectListItem { Value = ((int)BalanceType.Creditor).ToString(), Text = "دائن (ليه فلوس)" }
        };

        public List<SelectListItem> CustomerTypes { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = ((int)CustomerType.Retail).ToString(), Text = "قطاعي" },
            new SelectListItem { Value = ((int)CustomerType.Wholesale).ToString(), Text = "جملة" }
        };
    }
}
