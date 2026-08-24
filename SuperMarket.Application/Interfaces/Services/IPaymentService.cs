
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Payments;
using DomainPayment = SuperMarket.Domain.Enums.PaymentStatus;

namespace SuperMarket.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        #region Admin Operations

        Task<Result<Guid>> CreateAsync(
            PaymentCreateDto dto,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            PaymentUpdateDto dto,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<PaymentAdminDto>> GetByIdForAdminAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedResult<PaymentAdminDto>> GetPagedForAdminAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result> ChangeStatusAsync(
            Guid id,
            DomainPayment status,
            Guid performedBy,
            CancellationToken cancellationToken = default);

        #endregion

        #region Customer Operations

        Task<Result<PaymentCustomerDto>> GetByIdForCustomerAsync(
            Guid id,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<PagedResult<PaymentCustomerDto>> GetPagedByOrderIdAsync(
            Guid userId,
            Guid orderId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
