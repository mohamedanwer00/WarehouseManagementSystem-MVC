using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities;

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
        IProductWarehouseRepository ProductWarehouses { get; }
        int SaveChanges();
    }
}
