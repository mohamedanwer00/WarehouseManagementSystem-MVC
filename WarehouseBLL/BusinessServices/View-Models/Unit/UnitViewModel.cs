using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseBLL.BusinessServices.View_Models.Unit
{
    public class UnitViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Symbol { get; set; } = null!;
        public string? LastAction { get; set; }
    }
}