
using AutoMapper;
using SuperMarket.Application.DTOs.Categories;
using SuperMarket.Web.Areas.Admin.ViewModels.Categories;
using SuperMarket.Web.Areas.Customer.ViewModels.Categories;

namespace SuperMarket.Web.Mapping
{
    public sealed class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            // Create
            CreateMap<CategoryCreateViewModel, CategoryCreateDto>();
            // Update
            CreateMap<CategoryEditViewModel, CategoryUpdateDto>();
            // Admin List
            CreateMap<CategoryAdminDto, CategoryListItemViewModel>();
            // Admin Edit
            CreateMap<CategoryAdminDto, CategoryEditViewModel>();

            CreateMap<CategoryLookupDto,
          CustomerCategoryItemViewModel>()
    .ForMember(
        d => d.Title,
        o => o.MapFrom(s => s.Name));


            CreateMap<CategoryLookupDto, CategoryLookupViewModel>();



        }



    }
}

