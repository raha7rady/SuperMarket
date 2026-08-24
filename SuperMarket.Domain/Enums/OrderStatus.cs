namespace SuperMarket.Domain.Enums
{

    public enum OrderStatus : byte
    {
        /// <summary>
        /// Undefined state. Should never be used in valid domain flow.
        /// </summary>
        None = 0,

        /// <summary>
        /// Order has been created but not yet processed.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Order is being processed internally.
        /// </summary>
        Processing = 2,

        /// <summary>
        /// Order has been shipped to customer.
        /// </summary>
        Shipped = 3,

        /// <summary>
        /// Order has been delivered successfully.
        /// </summary>
        Delivered = 4,

        /// <summary>
        /// Order has been canceled.
        /// </summary>
        Canceled = 5,

        /// <summary>
        /// Order was returned after delivery.
        /// (Reserved for future extension)
        /// </summary>
        Returned = 6
    }
}
