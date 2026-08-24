
using DomainPayment = SuperMarket.Domain.Enums.PaymentStatus;
namespace SuperMarket.Application.DTOs.Payments
{
    public class PaymentAdminDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public DomainPayment Status { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
