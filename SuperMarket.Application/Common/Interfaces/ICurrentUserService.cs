namespace SuperMarket.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }          // AspNetUsers.Id

        Guid DomainUserId { get; }    // Users.Id

        string? Email { get; }

        bool IsAuthenticated { get; }
    }
}