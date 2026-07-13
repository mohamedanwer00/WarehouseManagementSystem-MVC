using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseBLL.FormViewModels.CashBox
{
    public class CashBoxTransactionFormViewModel
    {

            public int CashBoxId { get; set; }

            public string CashBoxName { get; set; } = null!;

            [Required(ErrorMessage = "المبلغ مطلوب")]
            [Range(50.00, double.MaxValue, ErrorMessage = "يجب أن يكون المبلغ أكبر من صفر")]
            public decimal Amount { get; set; }

            [Required(ErrorMessage = "البيان أو السبب مطلوب")]
            [StringLength(250, ErrorMessage = "البيان طويل جداً")]
            public string Notes { get; set; } = null!;
    }
}
