using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using WarehouseDAL.Data.Contexts;
using WarehouseDAL.Entities;
using WarehouseDAL.Entities.Transactions;
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
        public IOpeningStockRepository OpeningStocks { get; }
        public IPurchaseInvoiceRepository PurchaseInvoices { get; }
        public ISalesInvoiceRepository SalesInvoices { get; }
        public IProductWarehouseRepository ProductWarehouses { get; }

        // Transaction Repositories
        public IGenericRepository<SupplierTransaction> SupplierTransactions { get; }
        public IGenericRepository<CustomerTransaction> CustomerTransactions { get; }
        public IGenericRepository<InventoryTransaction> InventoryTransactions { get; }
        public IGenericRepository<CashTransaction> CashTransactions { get; }

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
            IOpeningStockRepository openingStocks,
            IPurchaseInvoiceRepository purchaseInvoices,
            IProductWarehouseRepository productWarehouses,
            ISalesInvoiceRepository salesInvoices,
            IGenericRepository<SupplierTransaction> supplierTransactions,
            IGenericRepository<CustomerTransaction> customerTransactions,
            IGenericRepository<InventoryTransaction> inventoryTransactions,
            IGenericRepository<CashTransaction> cashTransactions)
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
            OpeningStocks = openingStocks;
            PurchaseInvoices = purchaseInvoices;
            ProductWarehouses = productWarehouses;
            SalesInvoices = salesInvoices;
            SupplierTransactions = supplierTransactions;
            CustomerTransactions = customerTransactions;
            InventoryTransactions = inventoryTransactions;
            CashTransactions = cashTransactions;
        }

        public int SaveChanges() => _dbContext.SaveChanges();

        public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();


        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}