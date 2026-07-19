using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation
{
    public class CustomerRepository:GenericRepository<Customer>,ICustomerRepository
    {
        private readonly WarehouseDbContext _dbContext;

        public CustomerRepository(WarehouseDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
