using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDAL.Entities.Enums
{
    public enum InvoiceStatus
    {
        Draft = 1,//مسودة
        Paid = 2,//مدفوعة
        PartiallyPaid = 3,//مدفوعة جزئيا
        Cancelled = 4,//ملغاة
    }
}
