using DomainPayment = SuperMarket.Domain.Enums.PaymentStatus;
namespace SuperMarket.Application.DTOs.Payments
{
    public class PaymentCustomerDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public DomainPayment Status { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
