using WarehouseBLL.BusinessServices.View_Models.OpeningStock;
using WarehouseBLL.FormViewModels.OpeningStock;

namespace WarehouseBLL.Mapping
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            // Category

            #region Category
            TypeAdapterConfig<Category, CategoryViewModel>
                .NewConfig();

            TypeAdapterConfig<CategoryViewModel, Category>
                .NewConfig();

            TypeAdapterConfig<CategoryFormViewModel, Category>
                .NewConfig()
                .Ignore(dest => dest.Id);

            TypeAdapterConfig<Category, CategoryFormViewModel>
                .NewConfig();


            #endregion

            // User
            #region User
            TypeAdapterConfig<User, UserViewModel>
                .NewConfig();

            TypeAdapterConfig<UserViewModel, User>
                .NewConfig();

            TypeAdapterConfig<UserFormViewModel, User>
                .NewConfig()
                .Map(dest => dest.NormalizedEmail,
                     src => string.IsNullOrWhiteSpace(src.Email)
                            ? null
                            : src.Email.ToUpperInvariant())
                .Map(dest => dest.NormalizedUserName,
                     src => string.IsNullOrWhiteSpace(src.UserName)
                            ? null
                            : src.UserName.ToUpperInvariant());

            TypeAdapterConfig<User, UserFormViewModel>
                .NewConfig();
            #endregion

            // Branch
            #region Branch
            TypeAdapterConfig<Branch, BranchViewModel>
                .NewConfig()
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber);


            TypeAdapterConfig<BranchFormViewModel, Branch>
                .NewConfig()
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber);

            TypeAdapterConfig<Branch, BranchFormViewModel>
                .NewConfig();

            #endregion

            // Warehouse
            #region Warehouse
            TypeAdapterConfig<Warehouse, WarehouseViewModel>
                .NewConfig()
                .Map(dest => dest.BranchName, src => src.Branch != null ? src.Branch.Name : string.Empty);
            TypeAdapterConfig<WarehouseViewModel, Warehouse>
                .NewConfig();
            TypeAdapterConfig<WarehouseFormViewModel, Warehouse>
                .NewConfig()
                .Map(dest => dest.BranchId, src => src.SelectedBranch);

            TypeAdapterConfig<Warehouse, WarehouseFormViewModel>
                .NewConfig()
                .Map(dest => dest.SelectedBranch, src => src.BranchId);

            #endregion

            // CashBox
            #region CashBox
            TypeAdapterConfig<CashBox, CashBoxViewModel>
                .NewConfig()
                .Map(dest => dest.BranchName, src => src.Branch != null ? src.Branch.Name : string.Empty);

            TypeAdapterConfig<CashBoxFormViewModel, CashBox>
                .NewConfig()
                .Map(dest => dest.BranchId, src => src.SelectedBranch);

            TypeAdapterConfig<CashBox, CashBoxFormViewModel>
                .NewConfig()
                .Map(dest => dest.SelectedBranch, src => src.BranchId);
            TypeAdapterConfig<CashBox, CashBoxFormViewModel>
                .NewConfig()
                .Map(dest => dest.OpeningBalance, src => src.OpeningBalance);


            #endregion

            // Unit
            #region Unit
            TypeAdapterConfig<Unit, UnitViewModel>.NewConfig();
            TypeAdapterConfig<UnitViewModel, Unit>.NewConfig();
            TypeAdapterConfig<UnitFormViewModel, Unit>.NewConfig();
            TypeAdapterConfig<Unit, UnitFormViewModel>.NewConfig();
            #endregion

            //Product
            #region Product
            TypeAdapterConfig<Product, ProductViewModel>
                .NewConfig()
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : string.Empty)
                .Map(dest => dest.BaseUnitName, src => src.ProductUnits.FirstOrDefault(pu => pu.IsBaseUnit) != null
                    ? src.ProductUnits.FirstOrDefault(pu => pu.IsBaseUnit)!.Unit.Name : string.Empty)
                .Map(dest => dest.BaseUnitSellingPrice, src => src.ProductUnits.FirstOrDefault(pu => pu.IsBaseUnit) != null
                    ? src.ProductUnits.FirstOrDefault(pu => pu.IsBaseUnit)!.SellingPrice : 0);

            TypeAdapterConfig<ProductFormViewModel, Product>.NewConfig();
            TypeAdapterConfig<ProductUnitFormViewModel, ProductUnit>.NewConfig();

            TypeAdapterConfig<Product, ProductFormViewModel>.NewConfig();
            TypeAdapterConfig<ProductUnit, ProductUnitFormViewModel>.NewConfig();
            #endregion

            // Supplier
            #region Supplier
            TypeAdapterConfig<Supplier, SupplierViewModel>
                .NewConfig();

            TypeAdapterConfig<SupplierViewModel, Supplier>
                .NewConfig();

            TypeAdapterConfig<SupplierFormViewModel, Supplier>
                .NewConfig()
                .Ignore(dest => dest.Id);

            TypeAdapterConfig<Supplier, SupplierFormViewModel>
                .NewConfig();
            #endregion

            // Customer
            #region Customer

            TypeAdapterConfig<Customer, CustomerViewModel>
                .NewConfig();

            TypeAdapterConfig<CustomerViewModel, Customer>
                .NewConfig();

            TypeAdapterConfig<CustomerFormViewModel, Customer>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Map(dest => dest.CurrentBalance, src => src.OpeningBalance);
            TypeAdapterConfig<Customer, CustomerFormViewModel>
                .NewConfig();
            #endregion

            //OpeningStock
            #region OpeningStock
            TypeAdapterConfig<OpeningStock, OpeningStockViewModel>
                .NewConfig()
                .Map(dest => dest.BranchName, src => src.Warehouse.Branch.Name)
                .Map(dest => dest.WarehouseName, src => src.Warehouse.Name)
                .Map(dest => dest.ProductName, src => src.Product.Name);

            TypeAdapterConfig<OpeningStockViewModel, OpeningStock>
                .NewConfig();

            TypeAdapterConfig<OpeningStock, OpeningStockFormViewModel>
                .NewConfig()
                .Map(dest => dest.SelectedBranch, src => src.Warehouse.BranchId)
                .Map(dest => dest.SelectedWarehouse, src => src.WarehouseId);


            TypeAdapterConfig<OpeningStock, OpeningStockItemFormViewModel>
                .NewConfig()
                .Map(dest => dest.ProductId, src => src.ProductId)
                .Map(dest => dest.ProductName, src => src.Product.Name)
                .Map(dest => dest.Quantity, src => src.Quantity);

            TypeAdapterConfig<OpeningStockItemFormViewModel, OpeningStock>
                .NewConfig()
                .Map(dest => dest.ProductId, src => src.ProductId)
                .Map(dest => dest.Quantity, src => src.Quantity);

            #endregion
        }
    }
}
