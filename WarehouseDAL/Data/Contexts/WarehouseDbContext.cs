using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using WarehouseDAL.Entities;
using WarehouseDAL.Entities.Identity;

namespace WarehouseDAL.Data.Contexts
{
    public class WarehouseDbContext : IdentityDbContext<User, Role, int>
    {
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Category> Categories { get; set; } = null!;
    }
}
