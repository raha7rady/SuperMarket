
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Users;
using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.Interfaces.Services;

public interface IUserService
{
    #region Commands

    Task<Result<Guid>> CreateAsync(
        UserCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        UserUpdateDto dto,
        Guid performedBy,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid id,
        Guid deletedBy,
        CancellationToken cancellationToken = default);

    Task<Result> RestoreAsync(
        Guid id,
        Guid restoredBy,
        CancellationToken cancellationToken = default);

    Task<Result> ChangeRoleAsync(
        Guid id,
        UserRole role,
        Guid performedBy,
        CancellationToken cancellationToken = default);

    #endregion

    #region Queries

    Task<Result<UserAdminDto>> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<UserCustomerDto>> GetByIdForCustomerAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<UserAdminDto>> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<UserAdminDto>>> GetPagedForAdminAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<UserAdminDto>>> ListByRoleAsync(
        UserRole role,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<UserAdminDto>>> SearchAsync(
        string searchTerm,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    Task<Result<int>> CountAsync(
        CancellationToken cancellationToken = default);

    Task<Result<int>> CountByRoleAsync(
        UserRole role,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> ExistsByEmailAsync(
        string email,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default);

    #endregion
}