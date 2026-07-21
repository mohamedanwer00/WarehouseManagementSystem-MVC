using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities;

namespace WarehouseDAL.Entities
{
    public class CashBox :BaseEntity
    {
        public string Name { get; set; } = null!;
        public decimal OpeningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;
    }
}
