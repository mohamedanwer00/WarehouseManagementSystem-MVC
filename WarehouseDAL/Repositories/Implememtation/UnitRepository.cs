using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation
{
        public class UnitRepository : GenericRepository<Unit>, IUnitRepository
        {
        public UnitRepository(WarehouseDbContext context)
        : base(context)
        {
        }
    }
}
