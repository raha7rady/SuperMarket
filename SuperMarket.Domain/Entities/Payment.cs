using SuperMarket.Domain.Common;
using SuperMarket.Domain.Enums;
using SuperMarket.Domain.ValueObjects;

namespace SuperMarket.Domain.Entities
{
    public class Payment : AuditableEntity
    {
        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } = null!;

        public Toman Amount { get; private set; } = null!;
        public string PaymentMethod { get; private set; } = null!;
        public string? TransactionId { get; private set; }
        public string? Description { get; private set; }
        public PaymentStatus Status { get; private set; }

        private Payment() { }

        public Payment(
            Guid orderId,
            decimal amount,
            string paymentMethod,
            string? transactionId = null,
            string? description = null)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException("OrderId is required.", nameof(orderId));

            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new ArgumentException("PaymentMethod is required.", nameof(paymentMethod));

            OrderId = orderId;
            Amount = Toman.Create(amount);
            PaymentMethod = paymentMethod;
            TransactionId = transactionId;
            Description = description;
            Status = PaymentStatus.Pending;
        }

        public void UpdateDetails(
            decimal amount,
            string paymentMethod,
            string? transactionId,
            string? description,
            PaymentStatus status)
        {
            EnsureNotDeleted();

            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new ArgumentException("PaymentMethod is required.", nameof(paymentMethod));

            Amount = Toman.Create(amount);
            PaymentMethod = paymentMethod;
            TransactionId = transactionId;
            Description = description;
            Status = status;
        }

        public void MarkAsProcessing(Guid performedBy)
        {
            EnsureNotDeleted();

            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only a pending payment can start processing.");

            Status = PaymentStatus.Processing;
            SetModified(performedBy);
        }

        public void MarkAsPaid(string? transactionId, Guid performedBy)
        {
            EnsureNotDeleted();

            if (Status == PaymentStatus.Paid)
                throw new InvalidOperationException("Payment already paid.");

            if (Status is PaymentStatus.Refunded or PaymentStatus.Canceled)
                throw new InvalidOperationException("Payment cannot be marked as paid.");

            if (!string.IsNullOrWhiteSpace(transactionId))
                TransactionId = transactionId;

            Status = PaymentStatus.Paid;
            SetModified(performedBy);
        }

        public void MarkAsFailed(string? reason, Guid performedBy)
        {
            EnsureNotDeleted();

            if (Status == PaymentStatus.Paid)
                throw new InvalidOperationException("A paid payment cannot be marked as failed.");

            Status = PaymentStatus.Failed;

            if (!string.IsNullOrWhiteSpace(reason))
                Description = reason;

            SetModified(performedBy);
        }

        public void MarkAsRefunded(Guid performedBy)
        {
            EnsureNotDeleted();

            if (Status != PaymentStatus.Paid)
                throw new InvalidOperationException("Only a paid payment can be refunded.");

            Status = PaymentStatus.Refunded;
            SetModified(performedBy);
        }

        public void Cancel(Guid performedBy)
        {
            EnsureNotDeleted();

            if (Status == PaymentStatus.Paid)
                throw new InvalidOperationException("A paid payment cannot be canceled.");

            Status = PaymentStatus.Canceled;
            SetModified(performedBy);
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot modify a deleted payment.");
        }
    }
}
