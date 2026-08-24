using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Payments;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Interfaces.Repositories;
using DomainPayment = SuperMarket.Domain.Enums.PaymentStatus;

namespace SuperMarket.Application.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderService _orderService;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderService orderService,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _orderService = orderService;
        _unitOfWork = unitOfWork;
    }

    #region Admin Operations

    public async Task<Result<Guid>> CreateAsync(PaymentCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.OrderId == Guid.Empty)
            return Result<Guid>.Failure("OrderId is required.");

        if (dto.Amount <= 0)
            return Result<Guid>.Failure("Amount must be greater than zero.");

        Payment payment;

        try
        {
            payment = new Payment(dto.OrderId, dto.Amount, dto.PaymentMethod, dto.TransactionId, dto.Description);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(payment.Id);
    }

    public async Task<Result> UpdateAsync(PaymentUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(dto.Id, cancellationToken);

        if (payment is null || payment.IsDeleted)
            return Result.Failure("Payment not found.");

        try
        {
            payment.UpdateDetails(dto.Amount, dto.PaymentMethod, dto.TransactionId, dto.Description, dto.Status);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return Result.Failure(ex.Message);
        }

        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);

        if (payment is null || payment.IsDeleted)
            return Result.Failure("Payment not found.");

        await _paymentRepository.SoftDeleteAsync(id, Guid.Empty, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PaymentAdminDto>> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);

        if (payment is null || payment.IsDeleted)
            return Result<PaymentAdminDto>.Failure("Payment not found.");

        return Result<PaymentAdminDto>.Success(ToAdminDto(payment));
    }

    public async Task<PagedResult<PaymentAdminDto>> GetPagedForAdminAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var items = await _paymentRepository.ListPagedAsync<System.DateTimeOffset>(
            predicate: p => !p.IsDeleted,
            orderBy: p => p.CreatedDate,
            ascending: false,
            skip: (pageNumber - 1) * pageSize,
            take: pageSize,
            cancellationToken: cancellationToken);

        var totalCount = await _paymentRepository.CountAsync(p => !p.IsDeleted, cancellationToken);

        return PagedResult<PaymentAdminDto>.Success(
            items.Select(ToAdminDto).ToList(),
            pageNumber,
            pageSize,
            totalCount);
    }

    public async Task<Result> ChangeStatusAsync(Guid id, DomainPayment status, Guid performedBy, CancellationToken cancellationToken = default)
    {
        Result? result = null;

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);

            if (payment is null || payment.IsDeleted)
            {
                result = Result.Failure("Payment not found.");
                return;
            }

            try
            {
                ApplyStatus(payment, status, performedBy);
            }
            catch (InvalidOperationException ex)
            {
                result = Result.Failure(ex.Message);
                return;
            }

            await _paymentRepository.UpdateAsync(payment, cancellationToken);
            await _paymentRepository.SaveChangesAsync(cancellationToken);

            if (status == DomainPayment.Paid)
            {
                var orderResult = await _orderService.MarkAsPaidAsync(payment.OrderId, performedBy, cancellationToken);
                if (orderResult.IsFailure)
                    throw new InvalidOperationException(orderResult.FirstError);
            }
            else if (status == DomainPayment.Refunded)
            {
                var orderResult = await _orderService.MarkAsRefundedAsync(payment.OrderId, performedBy, cancellationToken);
                if (orderResult.IsFailure)
                    throw new InvalidOperationException(orderResult.FirstError);
            }

            result = Result.Success();
        },
        cancellationToken);

        return result ?? Result.Failure("Status change failed.");
    }

    #endregion

    #region Customer Operations

    public async Task<Result<PaymentCustomerDto>> GetByIdForCustomerAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdWithOrderAsync(id, cancellationToken);

        if (payment is null || payment.IsDeleted || payment.Order.UserId != userId)
            return Result<PaymentCustomerDto>.Failure("Payment not found.");

        return Result<PaymentCustomerDto>.Success(ToCustomerDto(payment));
    }

    public async Task<PagedResult<PaymentCustomerDto>> GetPagedByOrderIdAsync(Guid userId, Guid orderId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var order = await _orderService.GetByIdForCustomerAsync(orderId, userId, cancellationToken);

        if (order.IsFailure)
            return PagedResult<PaymentCustomerDto>.Failure("Order not found.");

        var items = await _paymentRepository.GetByOrderIdAsync(
            orderId,
            skip: (pageNumber - 1) * pageSize,
            take: pageSize,
            cancellationToken: cancellationToken);

        var totalCount = await _paymentRepository.CountByOrderIdAsync(orderId, cancellationToken);

        return PagedResult<PaymentCustomerDto>.Success(
            items.Select(ToCustomerDto).ToList(),
            pageNumber,
            pageSize,
            totalCount);
    }

    #endregion

    private static void ApplyStatus(Payment payment, DomainPayment status, Guid performedBy)
    {
        switch (status)
        {
            case DomainPayment.Processing:
                payment.MarkAsProcessing(performedBy);
                break;
            case DomainPayment.Paid:
                payment.MarkAsPaid(payment.TransactionId, performedBy);
                break;
            case DomainPayment.Failed:
                payment.MarkAsFailed(null, performedBy);
                break;
            case DomainPayment.Refunded:
                payment.MarkAsRefunded(performedBy);
                break;
            case DomainPayment.Canceled:
                payment.Cancel(performedBy);
                break;
            default:
                throw new InvalidOperationException("Unsupported status transition.");
        }
    }

    private static PaymentAdminDto ToAdminDto(Payment payment) => new()
    {
        Id = payment.Id,
        OrderId = payment.OrderId,
        Amount = payment.Amount.Amount,
        PaymentMethod = payment.PaymentMethod,
        TransactionId = payment.TransactionId ?? string.Empty,
        Status = payment.Status,
        Description = payment.Description,
        CreatedAt = payment.CreatedDate,
        UpdatedAt = payment.LastModifiedDate
    };

    private static PaymentCustomerDto ToCustomerDto(Payment payment) => new()
    {
        Id = payment.Id,
        Amount = payment.Amount.Amount,
        PaymentMethod = payment.PaymentMethod,
        Status = payment.Status,
        Description = payment.Description,
        CreatedAt = payment.CreatedDate
    };
}
