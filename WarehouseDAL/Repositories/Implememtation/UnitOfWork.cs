using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Repositories.Implememtation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WarehouseDbContext _dbContext;

        public ICategoryRepository Categories { get; }
        public IBranchRepository Branches { get; }
        public IWarehouseRepository Warehouses { get; }
        public ICashBoxRepository CashBoxes { get; }
        public IUnitRepository Units { get; }
        public IProductRepository Products { get; }
        public IProductUnitRepository ProductUnits { get; }
        public ISupplierRepository Suppliers { get; }
        public ICustomerRepository Customers { get; }
        public IPurchaseInvoiceRepository PurchaseInvoices { get; }
        public IProductWarehouseRepository ProductWarehouse { get; }
        public UnitOfWork(
            WarehouseDbContext dbContext,
            ICategoryRepository categories,
            IBranchRepository branches,
            IWarehouseRepository warehouses,
            ICashBoxRepository cashBoxes,
            IUnitRepository units,
            IProductRepository products,
            IProductUnitRepository productsUnits,
            ISupplierRepository suppliers,
            ICustomerRepository customers,
            IPurchaseInvoiceRepository purchaseInvoices,
            IProductWarehouseRepository productWarehouse)
        {
            _dbContext = dbContext;
            Categories = categories;
            Branches = branches;
            Warehouses = warehouses;
            CashBoxes = cashBoxes;
            Units = units;
            Products = products;
            ProductUnits = productsUnits;
            Suppliers = suppliers;
            Customers = customers;
            PurchaseInvoices = purchaseInvoices;
            ProductWarehouse = productWarehouse;
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}