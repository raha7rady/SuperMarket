using DomainPayment = SuperMarket.Domain.Enums.PaymentStatus;
namespace SuperMarket.Application.DTOs.Payments
{
    public class PaymentUpdateDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public string? Description { get; set; }
        public DomainPayment Status { get; set; }
    }
}
