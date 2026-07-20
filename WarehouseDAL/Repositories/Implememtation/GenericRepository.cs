using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly WarehouseDbContext _dbContext;

        public GenericRepository(WarehouseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? condition = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsNoTracking();

            if (condition != null)
                query = query.Where(condition);

            return query;
        }

        // 2. الدالة السحرية الجديدة: تتيح لك عمل Include و Querying مرن ومباشر من الـ Controller
        public IQueryable<TEntity> GetTableNoTracking()
        {
            return _dbContext.Set<TEntity>().AsNoTracking();
        }

        public async Task<TEntity?> GetById(int id) => await _dbContext.Set<TEntity>().FindAsync(id);

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return _dbContext.Set<TEntity>().AsQueryable();
        }
    }
}