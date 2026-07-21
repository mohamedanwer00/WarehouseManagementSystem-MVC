using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation
{
    public class OpeningStockRepository:GenericRepository<OpeningStock>,IOpeningStockRepository
    {
        private readonly WarehouseDbContext _dbContext;

        public OpeningStockRepository(WarehouseDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
