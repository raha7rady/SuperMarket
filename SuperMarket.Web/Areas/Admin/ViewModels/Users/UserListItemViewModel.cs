namespace SuperMarket.Web.Areas.Admin.ViewModels.Users
{
    public sealed class UserListItemViewModel
    {
        public Guid Id { get; init; }

        public string FullName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Role { get; init; } = "Customer";

        public int OrderCount { get; init; }

        public int CartItemCount { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public bool IsDeleted { get; init; }

        // Flags for UI actions
        public bool CanPromoteToAdmin => Role != "Admin" && !IsDeleted;
        public bool CanDemoteToCustomer => Role == "Admin" && !IsDeleted;
        public bool CanDelete => !IsDeleted;
        public bool CanRestore => IsDeleted;
    }
}