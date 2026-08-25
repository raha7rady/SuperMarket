
using AutoMapper;
using FluentValidation;
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Orders;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Domain.ValueObjects;

namespace SuperMarket.Application.Services
{
    public sealed class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateOrderDto> _createValidator;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IMapper mapper,
            IValidator<CreateOrderDto> createValidator)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _createValidator = createValidator;
        }

        #region Create & Delete

        public async Task<Result<Guid>> CreateAsync(
            CreateOrderDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                return Result<Guid>.Failure("Request cannot be null.");

            var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validation.IsValid)
                return validation.ToFailureResult<Guid>();

            if (dto.UserId == Guid.Empty)
                return Result<Guid>.Failure("UserId is required.");

            if (dto.Items is null || !dto.Items.Any())
                return Result<Guid>.Failure("Order must contain at least one item.");

            var order = new Order(dto.UserId);

            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                    return Result<Guid>.Failure("Item quantity must be greater than zero.");

                var product = await _productRepository
                    .GetByIdWithCategoryAsync(item.ProductId, cancellationToken);

                if (product is null || !product.IsActive)
                    return Result<Guid>.Failure($"Product {item.ProductId} is not available.");

                order.AddItem(
                    product.Id,
                    product.Title.Value,
                    product.Price.Amount,
                    item.Quantity,
                    dto.UserId);
            }

            if (dto.CheckoutDetails is not null)
            {
                var details = dto.CheckoutDetails;

                try
                {
                    var shippingAddress = ShippingAddress.Create(
                        details.RecipientFullName,
                        details.RecipientPhone,
                        details.Province,
                        details.City,
                        details.AddressLine,
                        details.PostalCode,
                        details.Plaque,
                        details.Unit,
                        details.DeliveryNote);

                    order.SetCheckoutDetails(
                        shippingAddress,
                        details.DeliveryOption,
                        details.PaymentMethod,
                        details.ShippingCost,
                        details.CouponCode,
                        details.CouponDiscount,
                        dto.UserId);
                }
                catch (ArgumentException ex)
                {
                    return Result<Guid>.Failure(ex.Message);
                }
            }

            await _orderRepository.AddAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(order.Id);
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            Guid performedBy,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result.Failure("OrderId is required.");

            var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

            if (order is null || order.IsDeleted)
                return Result.Failure("Order not found.");

            await _orderRepository.SoftDeleteAsync(id, performedBy, cancellationToken);

            return Result.Success();
        }

        #endregion

        #region Admin

        public async Task<Result<OrderAdminDto>> GetByIdForAdminAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result<OrderAdminDto>.Failure("OrderId is required.");

            var order = await _orderRepository.GetFullGraphAsync(id, cancellationToken);

            if (order is null || order.IsDeleted)
                return Result<OrderAdminDto>.Failure("Order not found.");

            var dto = _mapper.Map<OrderAdminDto>(order);

            return Result<OrderAdminDto>.Success(dto);
        }

        public async Task<PagedResult<OrderAdminDto>> GetPagedForAdminAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var orders = await _orderRepository.ListPagedWithIncludesAsync<DateTimeOffset>(
                predicate: o => !o.IsDeleted,
                orderBy: o => o.CreatedDate,
                ascending: false,
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                cancellationToken: cancellationToken,
                o => o.User,
                o => o.Items);

            var dtos = _mapper.Map<List<OrderAdminDto>>(orders);

            // No CountActiveAsync available; reusing ListActiveAsync for the count.
            var totalCount = (await _orderRepository.ListActiveAsync(0, int.MaxValue, cancellationToken)).Count;

            return PagedResult<OrderAdminDto>.Success(
                dtos,
                pageNumber,
                pageSize,
                totalCount);
        }

        #endregion

        #region Customer

        public async Task<Result<OrderCustomerDto>> GetByIdForCustomerAsync(
            Guid id,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty || userId == Guid.Empty)
                return Result<OrderCustomerDto>.Failure("Invalid request.");

            var order = await _orderRepository.GetOrderWithItemsAsync(id, cancellationToken);

            if (order is null || order.IsDeleted || order.UserId != userId)
                return Result<OrderCustomerDto>.Failure("Order not found.");

            var dto = _mapper.Map<OrderCustomerDto>(order);

            return Result<OrderCustomerDto>.Success(dto);
        }

        public async Task<PagedResult<OrderCustomerDto>> GetPagedForCustomerAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
                return PagedResult<OrderCustomerDto>.Failure("UserId is required.");

            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var orders = await _orderRepository.GetOrdersByUserIdWithItemsAsync(
                userId,
                skip: (pageNumber - 1) * pageSize,
                take: pageSize,
                cancellationToken: cancellationToken);

            var totalCount = await _orderRepository.CountByUserAsync(userId, cancellationToken);

            var dtos = _mapper.Map<List<OrderCustomerDto>>(orders);

            return PagedResult<OrderCustomerDto>.Success(
                dtos,
                pageNumber,
                pageSize,
                totalCount);
        }

        #endregion

        #region Items Management

        public async Task<Result> AddItemAsync(
            Guid orderId,
            OrderItemDto dto,
            Guid performedBy,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                return Result.Failure("Request cannot be null.");

            if (dto.Quantity <= 0)
                return Result.Failure("Quantity must be greater than zero.");

            var order = await _orderRepository.GetOrderWithItemsAsync(orderId, cancellationToken);

            if (order is null || order.IsDeleted)
                return Result.Failure("Order not found.");

            var product = await _productRepository
                .GetByIdWithCategoryAsync(dto.ProductId, cancellationToken);

            if (product is null || !product.IsActive)
                return Result.Failure("Product not available.");

            order.AddItem(
                product.Id,
                product.Title.Value,
                product.Price.Amount,
                dto.Quantity,
                performedBy);

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> RemoveItemAsync(
            Guid orderId,
            Guid productId,
            Guid performedBy,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(orderId, cancellationToken);

            if (order is null || order.IsDeleted)
                return Result.Failure("Order not found.");

            order.RemoveItem(productId, performedBy);

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> ChangeItemQuantityAsync(
            Guid orderId,
            Guid productId,
            int quantity,
            Guid performedBy,
            CancellationToken cancellationToken = default)
        {
            if (quantity <= 0)
                return Result.Failure("Quantity must be greater than zero.");

            var order = await _orderRepository.GetOrderWithItemsAsync(orderId, cancellationToken);

            if (order is null || order.IsDeleted)
                return Result.Failure("Order not found.");

            order.ChangeItemQuantity(productId, quantity, performedBy);

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        #endregion

        #region Order Status Management

        public async Task<Result> MarkAsPaidAsync(Guid orderId, Guid performedBy, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(orderId, cancellationToken);

            if (order is null || order.IsDeleted)
                return Result.Failure("Order not found.");

            order.MarkAsPaid(performedBy);

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> MarkAsShippedAsync(Guid orderId, Guid performedBy, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(orderId, cancellationToken);

            if (order is null || order.IsDeleted)
                return Result.Failure("Order not found.");

            order.MarkAsShipped(performedBy);

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> MarkAsDeliveredAsync(Guid orderId, Guid performedBy, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(orderId, cancellationToken);

            if (order is null || order.IsDeleted)
                return Result.Failure("Order not found.");

            order.MarkAsDelivered(performedBy);

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> CancelAsync(Guid orderId, Guid performedBy, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(orderId, cancellationToken);

            if (order is null || order.IsDeleted)
                return Result.Failure("Order not found.");

            order.Cancel(performedBy);

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> MarkAsRefundedAsync(Guid orderId, Guid performedBy, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(orderId, cancellationToken);

            if (order is null || order.IsDeleted)
                return Result.Failure("Order not found.");

            order.MarkAsRefunded(performedBy);

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> RestoreAsync(Guid orderId, Guid performedBy, CancellationToken cancellationToken = default)
        {
            await _orderRepository.RestoreAsync(orderId, performedBy, cancellationToken);
            return Result.Success();
        }

        #endregion
    }
}
