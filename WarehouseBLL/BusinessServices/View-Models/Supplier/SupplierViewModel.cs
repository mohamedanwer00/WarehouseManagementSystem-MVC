using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities.Enums;

namespace WarehouseBLL.BusinessServices.View_Models.Supplier
{
    public class SupplierViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal OpeningBalance { get; set; }
        public BalanceType OpeningBalanceType { get; set; }
        public decimal CurrentBalance { get; set; }
        public string? LastAction { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
