using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDAL.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public string? LastAction { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedById { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedById { get; set; }
    }
}
