

using AutoMapper;
using SuperMarket.Domain.Entities;
using SuperMarket.Application.DTOs.Users;
using SuperMarket.Application.DTOs.Products;
using SuperMarket.Application.DTOs.Categories;
using SuperMarket.Application.DTOs.Orders;
using SuperMarket.Application.DTOs.Cart;



namespace SuperMarket.Application.Mappings
{
    public sealed class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            ConfigureUserMappings();
            ConfigureProductMappings();
            ConfigureCategoryMappings();
            ConfigureCartMappings();
            ConfigureOrderMappings();
        }

        #region User

        private void ConfigureUserMappings()
        {
            CreateMap<User, UserAdminDto>()
                .ForMember(
                    d => d.FullName,
                    o => o.MapFrom(s =>
                        s.Name.ToString()))
                .ForMember(
                    d => d.Email,
                    o => o.MapFrom(s =>
                        s.Email != null
                            ? s.Email.Value
                            : string.Empty))
                .ForMember(
                    d => d.Role,
                    o => o.MapFrom(s => s.Role))
                .ForMember(
                    d => d.OrderCount,
                    o => o.Ignore())
                .ForMember(
                    d => d.CartItemCount,
                    o => o.Ignore())
                .ForMember(
                    d => d.CreatedAt,
                    o => o.MapFrom(s => s.CreatedDate))
                .ForMember(
                    d => d.UpdatedAt,
                    o => o.MapFrom(s => s.LastModifiedDate))
                .ForMember(
                    d => d.IsDeleted,
                    o => o.MapFrom(s => s.IsDeleted));

            CreateMap<User, UserCustomerDto>()
                .ForMember(
                    d => d.FullName,
                    o => o.MapFrom(s =>
                        s.Name.ToString()))
                .ForMember(
                    d => d.Email,
                    o => o.MapFrom(s =>
                        s.Email != null
                            ? s.Email.Value
                            : string.Empty));
        }

        #endregion

        #region Product

        private void ConfigureProductMappings()
        {
            CreateMap<Product, ProductAdminDto>()
                .ForMember(
                    d => d.Title,
                    o => o.MapFrom(s => s.Title.Value))
                .ForMember(
                    d => d.Price,
                    o => o.MapFrom(s => s.Price.Amount))
                .ForMember(
                    d => d.Stock,
                    o => o.MapFrom(s => s.Stock.Value))
                .ForMember(
                    d => d.DisplayOrder,
                    o => o.MapFrom(s => s.SortOrder.Value))
                .ForMember(
                    d => d.CategoryName,
                    o => o.MapFrom(s =>
                        s.Category != null
                            ? s.Category.Title
                            : string.Empty))
                .ForMember(
                    d => d.IsActive,
                    o => o.MapFrom(s => s.IsActive))
                .ForMember(
                    d => d.CreatedAt,
                    o => o.MapFrom(s => s.CreatedDate))
                .ForMember(
                    d => d.UpdatedAt,
                    o => o.MapFrom(s => s.LastModifiedDate));

            CreateMap<Product, ProductCustomerDto>()
                .ForMember(
                    d => d.Title,
                    o => o.MapFrom(s => s.Title.Value))
                .ForMember(
                    d => d.Price,
                    o => o.MapFrom(s => s.Price.Amount))
                .ForMember(
                    d => d.FinalPrice,
                    o => o.MapFrom(s => s.Price.Amount))
                .ForMember(
                    d => d.HasDiscount,
                    o => o.MapFrom(_ => false))
                .ForMember(
                    d => d.Stock,
                    o => o.MapFrom(s => s.Stock.Value))
                .ForMember(
                    d => d.ImageUrl,
                    o => o.MapFrom(s => s.ImageUrl))
                .ForMember(
                    d => d.Description,
                    o => o.MapFrom(s => s.Description))
                .ForMember(
                    d => d.Slug,
                    o => o.MapFrom(s => s.Slug))
                .ForMember(
                    d => d.CategoryName,
                    o => o.MapFrom(s =>
                        s.Category != null
                            ? s.Category.Title
                            : string.Empty));

            CreateMap<ProductCreateDto, Product>()
                .ConstructUsing(s =>
                    new Product(
                        s.Title,
                        s.Price,
                        s.Stock,
                        s.ImageUrl,
                        s.CategoryId,
                        s.Description,
                        s.Slug,
                        s.DisplayOrder));

            CreateMap<ProductUpdateDto, Product>()
                .ForAllMembers(o => o.Ignore());
        }

        #endregion

        #region Category

        private void ConfigureCategoryMappings()
        {
            CreateMap<Category, CategoryAdminDto>()
                .ForMember(
                    d => d.ProductCount,
                    o => o.MapFrom(s =>
                        s.Products != null
                            ? s.Products.Count
                            : 0))
                .ForMember(
                    d => d.Title,
                    o => o.MapFrom(s => s.Title))
                .ForMember(
                    d => d.DisplayOrder,
                    o => o.MapFrom(s => s.DisplayOrder))
                .ForMember(
                    d => d.IsActive,
                    o => o.MapFrom(s => s.IsActive))
                .ForMember(
                    d => d.CreatedAt,
                    o => o.MapFrom(s => s.CreatedDate))
                .ForMember(
                    d => d.UpdatedAt,
                    o => o.MapFrom(s => s.LastModifiedDate));

            CreateMap<Category, CategoryCustomerDto>()
                .ForMember(
                    d => d.Title,
                    o => o.MapFrom(s => s.Title))
                .ForMember(
                    d => d.DisplayOrder,
                    o => o.MapFrom(s => s.DisplayOrder));

            CreateMap<CategoryCreateDto, Category>()
                .ConstructUsing(s =>
                    new Category(
                        s.Title,
                        s.DisplayOrder));

            CreateMap<CategoryUpdateDto, Category>()
                .ForAllMembers(o => o.Ignore());
        }

        #endregion

        #region Cart

        private void ConfigureCartMappings()
        {
            CreateMap<Cart, CartAdminDto>()
                .ForMember(
                    d => d.UserName,
                    o => o.MapFrom(s =>
    s.User != null && s.User.Name != null
        ? $"{s.User.Name.FirstName} {s.User.Name.LastName}"
        : string.Empty))
                .ForMember(
                    d => d.TotalItems,
                    o => o.MapFrom(s => s.TotalItems))
                .ForMember(
                    d => d.TotalAmount,
                    o => o.MapFrom(s => s.TotalAmount.Amount))
                .ForMember(
                    d => d.Items,
                    o => o.MapFrom(s => s.Items));

            CreateMap<Cart, CartCustomerDto>()
                .ForMember(
                    d => d.TotalItems,
                    o => o.MapFrom(s => s.TotalItems))
                .ForMember(
                    d => d.TotalAmount,
                    o => o.MapFrom(s => s.TotalAmount.Amount))
                .ForMember(
                    d => d.Items,
                    o => o.MapFrom(s => s.Items));

            CreateMap<CartItem, CartItemDetailDto>()
                .ForMember(
                    d => d.ProductTitle,
                    o => o.MapFrom(s => s.Title.Value))
                .ForMember(
                    d => d.Price,
                    o => o.MapFrom(s => s.Price.Amount))
                .ForMember(
                    d => d.Quantity,
                    o => o.MapFrom(s => s.Quantity.Value))
                .ForMember(
                    d => d.ProductImageUrl,
                    o => o.MapFrom(s =>
                        s.Product != null
                            ? s.Product.ImageUrl
                            : string.Empty));
        }

        #endregion

        #region Order

        private void ConfigureOrderMappings()
        {
            CreateMap<Order, OrderAdminDto>()
                .ForMember(
                    d => d.UserName,
                    o => o.MapFrom(s =>
                        s.User != null &&
                        s.User.Name != null
                            ? s.User.Name.ToString()
                            : string.Empty))
                .ForMember(
                    d => d.TotalPrice,
                    o => o.MapFrom(s => s.TotalPrice.Amount))
                .ForMember(
                    d => d.OrderStatus,
                    o => o.MapFrom(s => s.OrderStatus.ToString()))
                .ForMember(
                    d => d.PaymentStatus,
                    o => o.MapFrom(s => s.PaymentStatus.ToString()))
                .ForMember(
                    d => d.Items,
                    o => o.MapFrom(s => s.Items));

            CreateMap<Order, OrderCustomerDto>()
                .ForMember(
                    d => d.TotalPrice,
                    o => o.MapFrom(s => s.TotalPrice.Amount))
                .ForMember(
                    d => d.OrderStatus,
                    o => o.MapFrom(s => s.OrderStatus.ToString()))
                .ForMember(
                    d => d.PaymentStatus,
                    o => o.MapFrom(s => s.PaymentStatus.ToString()))
                .ForMember(
                    d => d.Items,
                    o => o.MapFrom(s => s.Items));

            CreateMap<OrderItem, OrderItemDetailDto>()
                .ForMember(
                    d => d.ProductTitle,
                    o => o.MapFrom(s => s.Title.Value))
                .ForMember(
                    d => d.Price,
                    o => o.MapFrom(s => s.Price.Amount))
                .ForMember(
                    d => d.Quantity,
                    o => o.MapFrom(s => s.Quantity.Value));

            CreateMap<CreateOrderDto, Order>()
                .ConstructUsing(s =>
                    new Order(s.UserId));
        }

        #endregion
    }
}

