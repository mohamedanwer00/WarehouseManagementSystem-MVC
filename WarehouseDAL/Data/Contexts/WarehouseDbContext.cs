using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using WarehouseDAL.Entities;
using WarehouseDAL.Entities.Identity;
using WarehouseDAL.Entities.Transactions;
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
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductUnit> ProductUnits { get; set; } = null!;
        public DbSet<Unit> Units { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<CashBox> CashBoxes { get; set; } = null!;
        public DbSet<Warehouse> Warehouses { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<OpeningStock> OpeningStock { get; set; } = null!;
        public DbSet<CashTransaction> CashsTransactions { get; set; } = null!;
        public DbSet<CustomerTransaction> CustomerTransactions { get; set; }=null!;
        public DbSet<SupplierTransaction> SupplierTransactions { get; set; } = null!;
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }= null!;
    }
}
