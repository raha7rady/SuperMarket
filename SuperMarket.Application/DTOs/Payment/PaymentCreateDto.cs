namespace SuperMarket.Application.DTOs.Payments
{
    public class PaymentCreateDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public string? Description { get; set; }
    }
}
