using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseBLL.BusinessServices.View_Models.Product
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public decimal MinimumQuantity { get; set; }
        public decimal MaximumQuantity { get; set; }
        public string CategoryName { get; set; } = null!;

        public string BaseUnitName { get; set; } = null!;
        public decimal BaseUnitSellingPrice { get; set; }

        public string? LastAction { get; set; }

        public ICollection<ProductUnitViewModel> ProductUnits { get; set; } = new List<ProductUnitViewModel>();
    }
}
