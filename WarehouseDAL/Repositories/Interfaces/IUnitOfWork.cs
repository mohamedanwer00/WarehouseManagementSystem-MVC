using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using WarehouseDAL.Entities;
using WarehouseDAL.Entities.Transactions;

namespace WarehouseDAL.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IBranchRepository Branches { get; }
        IWarehouseRepository Warehouses { get; }
        ICashBoxRepository CashBoxes { get; }
        IUnitRepository Units { get; }
        IProductRepository Products { get; }
        IProductUnitRepository ProductUnits { get; }
        ISupplierRepository Suppliers { get; }
        ICustomerRepository Customers { get; }
        IOpeningStockRepository OpeningStocks { get; }
        IPurchaseInvoiceRepository PurchaseInvoices { get; }
        ISalesInvoiceRepository SalesInvoices { get; }
        IProductWarehouseRepository ProductWarehouses { get; }

        // Transaction Repositories
        IGenericRepository<SupplierTransaction> SupplierTransactions { get; }
        IGenericRepository<CustomerTransaction> CustomerTransactions { get; }
        IGenericRepository<InventoryTransaction> InventoryTransactions { get; }
        IGenericRepository<CashTransaction> CashTransactions { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
