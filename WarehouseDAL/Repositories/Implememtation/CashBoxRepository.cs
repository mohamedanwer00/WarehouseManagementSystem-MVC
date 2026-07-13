using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation
{
    public class CashBoxRepository :GenericRepository<CashBox>, ICashBoxRepository
    {
        private readonly WarehouseDbContext _dbContext;

        public CashBoxRepository(WarehouseDbContext dbContext): base(dbContext) 
        {
            _dbContext = dbContext;
        }
    }
}
