using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseBLL.BusinessServices.View_Models.CashBox
{
    public class CashBoxViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal CurrentBalance { get; set; }
        public string? BranchName { get; set; }
        public string? LastAction { get; set; }
    }
}
