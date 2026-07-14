using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation
{
    public class ProductRepository:GenericRepository<Product>,IProductRepository
    {
        private readonly WarehouseDbContext _dbContext;

        public ProductRepository(WarehouseDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
