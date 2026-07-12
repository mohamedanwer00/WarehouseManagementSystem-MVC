using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation
{
    public class BranchRepository : GenericRepository<Branch>, IBranchRepository
    {
        private readonly WarehouseDbContext _dbContext;

        public BranchRepository(WarehouseDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
