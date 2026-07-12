using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDAL.Entities
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal OpeningBalance { get; set; }//الرصيد الافتتاحي
        public string? Note { get; set; }

    }
}
