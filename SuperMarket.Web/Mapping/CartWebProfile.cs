using AutoMapper;

using SuperMarket.Application.DTOs.Cart;

using SuperMarket.Web.Areas.Customer.ViewModels.Cart;

namespace SuperMarket.Web.Mappings.Cart;

public sealed class CartWebProfile : Profile
{
    public CartWebProfile()
    {
        ConfigureCartMappings();
    }

    private void ConfigureCartMappings()
    {
        CreateMap<CartCustomerDto, CartViewModel>()
            .ForMember(
                d => d.CartId,
                o => o.MapFrom(s => s.Id))
            .ForMember(
                d => d.TotalItems,
                o => o.MapFrom(s => s.TotalItems))
            .ForMember(
                d => d.TotalAmount,
                o => o.MapFrom(s => s.TotalAmount))
            .ForMember(
                d => d.Items,
                o => o.MapFrom(s => s.Items));



        CreateMap<CartItemDetailDto, CartItemViewModel>()
            .ForMember(
                d => d.ProductId,
                o => o.MapFrom(s => s.ProductId))
            .ForMember(
                d => d.ProductTitle,
                o => o.MapFrom(s => s.ProductTitle))
            .ForMember(
                d => d.ProductImageUrl,
                o => o.MapFrom(s => s.ProductImageUrl))
            .ForMember(
                d => d.UnitPrice,
                o => o.MapFrom(s => s.Price))
            .ForMember(
                d => d.Quantity,
                o => o.MapFrom(s => s.Quantity))
            .ForMember(
                d => d.SubTotal,
                o => o.MapFrom(s => s.SubTotal));

        CreateMap<AddToCartViewModel, CartItemDto>()
            .ForMember(
                d => d.ProductId,
                o => o.MapFrom(s => s.ProductId))
            .ForMember(
                d => d.Quantity,
                o => o.MapFrom(s => s.Quantity));

        CreateMap<UpdateCartItemViewModel, CartUpdateItemDto>()
            .ForMember(
                d => d.ProductId,
                o => o.MapFrom(s => s.ProductId))
            .ForMember(
                d => d.Quantity,
                o => o.MapFrom(s => s.Quantity));
    }
}