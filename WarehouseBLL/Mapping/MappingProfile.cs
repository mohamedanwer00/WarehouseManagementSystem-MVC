using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseBLL.BusinessServices.View_Models;
using WarehouseBLL.BusinessServices.View_Models.Branch;
using WarehouseBLL.BusinessServices.View_Models.CashBox;
using WarehouseBLL.BusinessServices.View_Models.Unit;
using WarehouseBLL.BusinessServices.View_Models.Users;
using WarehouseBLL.BusinessServices.View_Models.Warehouse;
using WarehouseBLL.FormViewModels.Branch;
using WarehouseBLL.FormViewModels.CashBox;
using WarehouseBLL.FormViewModels.Category;
using WarehouseBLL.FormViewModels.Unit;
using WarehouseBLL.FormViewModels.User;
using WarehouseBLL.FormViewModels.Warehouse;
using WarehouseDAL.Entities;
using WarehouseDAL.Entities.Entities;
using WarehouseDAL.Entities.Identity;

namespace WarehouseBLL.Mapping
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            // Category

            #region Category
            TypeAdapterConfig<Category, CategoryViewModel>
    .NewConfig()
    .Ignore(dest => dest.LastAction);

            TypeAdapterConfig<CategoryViewModel, Category>
                .NewConfig();

            TypeAdapterConfig<Category, CategoryFormViewModel>
                .NewConfig();

            TypeAdapterConfig<CategoryFormViewModel, Category>
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
            TypeAdapterConfig<UnitFormViewModel, Unit>.NewConfig();
            TypeAdapterConfig<Unit, UnitFormViewModel>.NewConfig();
            #endregion
        }
    }
}
