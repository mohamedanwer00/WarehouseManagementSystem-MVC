using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseBLL.BusinessServices.View_Models;
using WarehouseBLL.BusinessServices.View_Models.Users;
using WarehouseBLL.FormViewModels.Category;
using WarehouseBLL.FormViewModels.User;
using WarehouseDAL.Entities;
using WarehouseDAL.Entities.Identity;

namespace WarehouseBLL.Mapping
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            // Category

            TypeAdapterConfig<Category, CategoryViewModel>
                .NewConfig()
                .Ignore(dest => dest.LastAction);

            TypeAdapterConfig<CategoryViewModel, Category>
                .NewConfig();

            TypeAdapterConfig<Category, CategoryFormViewModel>
                .NewConfig();

            TypeAdapterConfig<CategoryFormViewModel, Category>
                .NewConfig();
            // User
            //TypeAdapterConfig<User, UserViewModel>
            //    .NewConfig();

            //TypeAdapterConfig<UserViewModel, User>
            //    .NewConfig();

            //TypeAdapterConfig<UserFormViewModel, User>
            //    .NewConfig()
            //    .Map(dest => dest.NormalizedEmail,
            //         src => src.Email.ToUpper())
            //    .Map(dest => dest.NormalizedUserName,
            //         src => src.UserName.ToUpper());

            //TypeAdapterConfig<User, UserFormViewModel>
            //    .NewConfig();

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
        }
    }
    //public class MappingProfile
    //{
    //    //public MappingProfile()
    //    //{
    //    //    MapCategory();
    //    //}

    //    //private void MapCategory()
    //    //{
    //    //    CreateMap<Category,CategoryViewModel>()
    //    //        .ForMember(dest => dest.LastAction, opt => opt.Ignore())
    //    //        .ReverseMap();
    //    //    CreateMap<Category, CategoryFormViewModel>()
    //    //        .ReverseMap();

    //    //}


    //}
}
