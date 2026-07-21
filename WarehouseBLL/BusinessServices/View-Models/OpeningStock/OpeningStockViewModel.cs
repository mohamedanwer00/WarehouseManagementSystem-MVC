using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseBLL.BusinessServices.View_Models.OpeningStock
{
    public class OpeningStockViewModel
    {
        public int Id { get; set; }
        public string BranchName { get; set; } = null!;
        public string WarehouseName { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal Quantity { get; set; }
    }
}
