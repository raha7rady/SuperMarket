
using AutoMapper;

using SuperMarket.Application.DTOs.Products;
using SuperMarket.Application.Products.Queries;

using SuperMarket.Web.Areas.Admin.ViewModels.Products;

using SuperMarket.Web.Areas.Customer.ViewModels.Products;

namespace SuperMarket.Web.Mappings.Products;

public sealed class ProductWebProfile : Profile
{
    public ProductWebProfile()
    {
        ConfigureAdminMappings();

        ConfigureCustomerMappings();
    }

    // =========================================
    // Admin
    // =========================================

    private void ConfigureAdminMappings()
    {
        CreateMap<ProductAdminDto, ProductListItemViewModel>();

        CreateMap<ProductAdminDto, ProductDetailsViewModel>();

        CreateMap<ProductAdminDto, ProductEditViewModel>()
            .ForMember(
                d => d.Categories,
                o => o.Ignore());

        CreateMap<ProductCreateViewModel, ProductCreateDto>();

        CreateMap<ProductEditViewModel, ProductUpdateDto>();
    }

    // =========================================
    // Customer
    // =========================================

    private void ConfigureCustomerMappings()
    {
        CreateMap<ProductCustomerDto,
            CustomerProductCardViewModel>();

        CreateMap<ProductCustomerDto,
            CustomerProductListItemViewModel>();

        CreateMap<ProductCustomerDto,
            CustomerProductDetailsViewModel>();

        CreateMap<CustomerProductFilterViewModel,
            ProductCustomerQuery>();

    }
}