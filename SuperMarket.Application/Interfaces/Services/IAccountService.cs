
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Account;
using SuperMarket.Application.DTOs.Users;

namespace SuperMarket.Application.Interfaces.Services;

public interface IAccountService
{
    // Self-service: creates domain User + Identity user atomically
    Task<Result<RegisterResultDto>> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default);

    // Self-service: validates credentials, handles lockout, returns rich result
    Task<Result<LoginResultDto>> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default);

    // Self-service: signs out the current session
    Task LogoutAsync();

    // Self-service: verifies current password before allowing change
    Task<Result> ChangePasswordAsync(
        ChangePasswordDto dto,
        Guid domainUserId,
        CancellationToken cancellationToken = default);

    // Self-service: generates a secure token and sends a reset email
    Task<Result> ForgotPasswordAsync(
        ForgotPasswordDto dto,
        string resetPasswordBaseUrl,
        CancellationToken cancellationToken = default);

    // Self-service: validates the emailed token and resets the password
    Task<Result> ResetPasswordAsync(
        ResetPasswordDto dto,
        CancellationToken cancellationToken = default);

    // Admin: force-resets a user's password without a token — updates both stores
    Task<Result> AdminResetPasswordAsync(
        UserResetPasswordDto dto,
        Guid performedBy,
        CancellationToken cancellationToken = default);

    // Self-service: validates the emailed confirmation token and marks the
    // Identity account's email as confirmed.
    Task<Result> ConfirmEmailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default);

    // Self-service: generates a new confirmation token and re-sends the
    // confirmation email. Always succeeds regardless of whether the email
    // exists or is already confirmed, to prevent account enumeration.
    Task<Result> ResendConfirmationEmailAsync(
        string email,
        string confirmationBaseUrl,
        CancellationToken cancellationToken = default);
}