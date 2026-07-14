using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseBLL.FormViewModels.CashBox
{
    public class CashBoxTransactionFormViewModel
    {

        public int Id { get; set; }

        public string? Name { get; set; }

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(0.00, double.MaxValue, ErrorMessage = "يجب أن يكون المبلغ أكبر من صفر")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "البيان أو السبب مطلوب")]
        [StringLength(250, ErrorMessage = "البيان طويل جداً")]
        public string? Notes { get; set; }
    }
}
