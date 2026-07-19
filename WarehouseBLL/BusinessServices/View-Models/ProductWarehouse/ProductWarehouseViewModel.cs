using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseBLL.BusinessServices.View_Models.ProductWarehouse
{
    public class ProductWarehouseViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public int UnitId { get; set; }
        public string UnitName { get; set; }= null!;
        public decimal Quantity { get; set; }
    }
}
