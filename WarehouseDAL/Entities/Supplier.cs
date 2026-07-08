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
        public string TaxNumber { get; set; } = null!;//رقم الضريبة

        public string CommercialRegisterNumber { get; set; } = null!;//رقم السجل التجاري

        public decimal OpeningBalance { get; set; }//الرصيد الافتتاحي
        public string? Note { get; set; }

        public string Description { get; set; }= null!;
    }
}
