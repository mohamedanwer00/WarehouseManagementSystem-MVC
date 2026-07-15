using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseBLL.BusinessServices.View_Models.Product
{
    public class ProductUnitViewModel
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public decimal Factor { get; set; }
        public bool IsBaseUnit { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }

        // ميزة احترافية إضافية: حساب نسبة ربح الوحدة تلقائياً!
        public decimal ProfitMargin => SellingPrice - PurchasePrice;
        public decimal ProfitPercentage => PurchasePrice > 0 ? (ProfitMargin / PurchasePrice) * 100 : 0;
    }
}
