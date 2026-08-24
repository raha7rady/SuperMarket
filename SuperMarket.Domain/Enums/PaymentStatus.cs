namespace SuperMarket.Domain.Enums
{
    /// <summary>
    /// Represents the payment state of an Order.
    /// Designed for extensibility and integration with payment gateways.
    /// </summary>
    public enum PaymentStatus : byte
    {
        /// <summary>
        /// Undefined payment state.
        /// </summary>
        None = 0,

        /// <summary>
        /// Payment has not yet been attempted.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Payment is currently being processed.
        /// </summary>
        Processing = 2,

        /// <summary>
        /// Payment completed successfully.
        /// </summary>
        Paid = 3,

        /// <summary>
        /// Payment attempt failed.
        /// </summary>
        Failed = 4,

        /// <summary>
        /// Payment was refunded.
        /// </summary>
        Refunded = 5,

        /// <summary>
        /// Payment was canceled by user or system.
        /// </summary>
        Canceled = 6
    }
}
