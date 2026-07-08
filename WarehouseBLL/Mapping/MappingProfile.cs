using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseBLL.BusinessServices.View_Models;
using WarehouseBLL.FormViewModels;
using WarehouseDAL.Entities;

namespace WarehouseBLL.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            MapCategory();
        }

        private void MapCategory()
        {
            CreateMap<Category,CategoryViewModel>()
                .ForMember(dest => dest.LastAction, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<Category, CategoryFormViewModel>()
                .ReverseMap();

        }
    }
}
