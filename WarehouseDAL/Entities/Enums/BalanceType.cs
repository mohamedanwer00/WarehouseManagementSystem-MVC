using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDAL.Entities.Enums
{
    public enum BalanceType
    {
        Creditor = 1, // دائن (المورد له فلوس عندنا)
        Debitor = 2  // مدين (احنا لنا فلوس عند المورد - حالة نادرة بس بتحصل)
    }
}
