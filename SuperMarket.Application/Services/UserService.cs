

using AutoMapper;
using FluentValidation;
using SuperMarket.Application.Common;
using SuperMarket.Application.Common.Extensions;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.DTOs.Users;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;
using SuperMarket.Domain.Interfaces.Repositories;

namespace SuperMarket.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IIdentitySyncService _identitySyncService;
    private readonly IMapper _mapper;
    private readonly IValidator<UserCreateDto> _createValidator;
    private readonly IValidator<UserUpdateDto> _updateValidator;

    public UserService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IIdentitySyncService identitySyncService,
        IMapper mapper,
        IValidator<UserCreateDto> createValidator,
        IValidator<UserUpdateDto> updateValidator)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(passwordHasher);
        ArgumentNullException.ThrowIfNull(identitySyncService);
        ArgumentNullException.ThrowIfNull(mapper);

        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _identitySyncService = identitySyncService;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    #region Commands

    public async Task<Result<Guid>> CreateAsync(
        UserCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return validation.ToFailureResult<Guid>();

        if (await _userRepository.ExistsByEmailAsync(
                dto.Email,
                cancellationToken: cancellationToken))
        {
            return Result<Guid>.Failure("Email is already in use.");
        }

        if (!dto.Role.IsValid())
        {
            return Result<Guid>.Failure("Invalid user role.");
        }

        var passwordHash =
            _passwordHasher.HashPassword(dto.Password);

        var user = new User(
            dto.FirstName,
            dto.LastName,
            dto.Email,
            passwordHash);

        user.ChangeRole(dto.Role);


        try
        {
            await _unitOfWork.ExecuteTransactionAsync(async () =>
            {
                await _userRepository.AddAsync(
                    user,
                    cancellationToken);


                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);


                var identityResult =
                    await _identitySyncService.CreateIdentityUserAsync(
                        user.Id,
                        dto.Email,
                        dto.Password,
                        dto.Role,
                        cancellationToken);


                if (identityResult.IsFailure)
                {
                    throw new InvalidOperationException(
                        string.Join(
                            ", ",
                            identityResult.Errors));
                }


            },
            cancellationToken);


            return Result<Guid>.Success(user.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }

    public async Task<Result> UpdateAsync(
        UserUpdateDto dto,
        Guid performedBy,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return validation.ToResult();

        var user = await _userRepository.GetByIdAsync(
            dto.Id,
            cancellationToken);


        if (user is null)
        {
            return Result.Failure("User not found.");
        }


        if (await _userRepository.ExistsByEmailAsync(
                dto.Email,
                dto.Id,
                cancellationToken))
        {
            return Result.Failure("Email is already in use.");
        }


        if (!dto.Role.IsValid())
        {
            return Result.Failure("Invalid user role.");
        }


        var emailChanged = !string.Equals(
            user.Email.Value,
            dto.Email.Trim().ToLowerInvariant(),
            StringComparison.Ordinal);


        var roleChanged = user.Role != dto.Role;



        user.ChangeName(
            dto.FirstName,
            dto.LastName,
            performedBy);


        user.ChangeEmail(
            dto.Email,
            performedBy);


        user.ChangeRole(
            dto.Role,
            performedBy);



        try
        {
            await _unitOfWork.ExecuteTransactionAsync(async () =>
            {

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);



                if (emailChanged)
                {
                    var emailSyncResult =
                        await _identitySyncService.SyncEmailAsync(
                            user.Id,
                            dto.Email,
                            cancellationToken);


                    if (emailSyncResult.IsFailure)
                    {
                        throw new InvalidOperationException(
                            string.Join(
                                ", ",
                                emailSyncResult.Errors));
                    }
                }



                if (roleChanged)
                {
                    var roleSyncResult =
                        await _identitySyncService.SyncRoleAsync(
                            user.Id,
                            dto.Role,
                            cancellationToken);


                    if (roleSyncResult.IsFailure)
                    {
                        throw new InvalidOperationException(
                            string.Join(
                                ", ",
                                roleSyncResult.Errors));
                    }
                }


            },
            cancellationToken);



            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(
        Guid id,
        Guid deletedBy,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure("User not found.");
        }

        user.SoftDelete(deletedBy);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RestoreAsync(
        Guid id,
        Guid restoredBy,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetIncludingDeletedByIdAsync(
            id,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure("User not found.");
        }

        user.Restore(restoredBy);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ChangeRoleAsync(
        Guid id,
        UserRole role,
        Guid performedBy,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            id,
            cancellationToken);


        if (user is null)
        {
            return Result.Failure("User not found.");
        }


        if (!role.IsValid())
        {
            return Result.Failure("Invalid user role.");
        }



        user.ChangeRole(
            role,
            performedBy);



        try
        {
            await _unitOfWork.ExecuteTransactionAsync(async () =>
            {

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);



                var syncResult =
                    await _identitySyncService.SyncRoleAsync(
                        user.Id,
                        role,
                        cancellationToken);



                if (syncResult.IsFailure)
                {
                    throw new InvalidOperationException(
                        string.Join(
                            ", ",
                            syncResult.Errors));
                }


            },
            cancellationToken);



            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    #endregion

    #region Queries

    public async Task<Result<UserAdminDto>> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetFullGraphAsync(
            id,
            cancellationToken);

        if (user is null)
        {
            return Result<UserAdminDto>.Failure("User not found.");
        }

        var dto = await MapToAdminDtoWithCountsAsync(user, cancellationToken);

        return Result<UserAdminDto>.Success(dto);
    }

    public async Task<Result<UserCustomerDto>> GetByIdForCustomerAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (user is null)
        {
            return Result<UserCustomerDto>.Failure("User not found.");
        }

        var dto = _mapper.Map<UserCustomerDto>(user);

        return Result<UserCustomerDto>.Success(dto);
    }

    public async Task<Result<UserAdminDto>> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var user = await _userRepository.GetByEmailAsync(
            email,
            cancellationToken);

        if (user is null)
        {
            return Result<UserAdminDto>.Failure("User not found.");
        }

        var dto = await MapToAdminDtoWithCountsAsync(user, cancellationToken);

        return Result<UserAdminDto>.Success(dto);
    }

    public async Task<Result<PagedResult<UserAdminDto>>> GetPagedForAdminAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var skip = (pageNumber - 1) * pageSize;

        var users = await _userRepository.ListPagedAsync(
            orderBy: u => u.CreatedDate,
            ascending: false,
            skip: skip,
            take: pageSize,
            cancellationToken: cancellationToken);

        var totalCount = await _userRepository.CountAsync(
            cancellationToken: cancellationToken);

        var items = await MapToAdminDtosWithCountsAsync(users, cancellationToken);

        var result = PagedResult<UserAdminDto>.Success(
            items,
            pageNumber,
            pageSize,
            totalCount);

        return Result<PagedResult<UserAdminDto>>.Success(result);
    }

    public async Task<Result<IReadOnlyList<UserAdminDto>>> ListByRoleAsync(
        UserRole role,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(role) || role == UserRole.None)
        {
            return Result<IReadOnlyList<UserAdminDto>>
                .Failure("Invalid user role.");
        }

        var users = await _userRepository.ListByRoleAsync(
            role,
            skip,
            take,
            cancellationToken);

        var items = await MapToAdminDtosWithCountsAsync(users, cancellationToken);

        return Result<IReadOnlyList<UserAdminDto>>
            .Success(items);
    }

    public async Task<Result<IReadOnlyList<UserAdminDto>>> SearchAsync(
        string searchTerm,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm);

        var users = await _userRepository.SearchAsync(
            searchTerm,
            skip,
            take,
            cancellationToken);

        var items = await MapToAdminDtosWithCountsAsync(users, cancellationToken);

        return Result<IReadOnlyList<UserAdminDto>>
            .Success(items);
    }

    public async Task<Result<int>> CountAsync(
        CancellationToken cancellationToken = default)
    {
        var count = await _userRepository.CountAsync(
            cancellationToken: cancellationToken);

        return Result<int>.Success(count);
    }

    public async Task<Result<int>> CountByRoleAsync(
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(role) || role == UserRole.None)
        {
            return Result<int>.Failure("Invalid user role.");
        }

        var count = await _userRepository.CountByRoleAsync(
            role,
            cancellationToken);

        return Result<int>.Success(count);
    }

    public async Task<Result<bool>> ExistsByEmailAsync(
        string email,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var exists = await _userRepository.ExistsByEmailAsync(
            email,
            excludeUserId,
            cancellationToken);

        return Result<bool>.Success(exists);
    }

    #endregion

    #region Activity counts (OrderCount / CartItemCount)

    private async Task<UserAdminDto> MapToAdminDtoWithCountsAsync(
        User user,
        CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<UserAdminDto>(user);

        var counts = await _userRepository.GetOrderAndCartCountsAsync(
            new[] { user.Id },
            cancellationToken);

        return ApplyCounts(dto, counts);
    }

    private async Task<IReadOnlyList<UserAdminDto>> MapToAdminDtosWithCountsAsync(
        IReadOnlyList<User> users,
        CancellationToken cancellationToken)
    {
        var dtos = _mapper.Map<IReadOnlyList<UserAdminDto>>(users);

        if (dtos.Count == 0)
        {
            return dtos;
        }

        var counts = await _userRepository.GetOrderAndCartCountsAsync(
            users.Select(u => u.Id),
            cancellationToken);

        return dtos
            .Select(dto => ApplyCounts(dto, counts))
            .ToList();
    }

    private static UserAdminDto ApplyCounts(
        UserAdminDto dto,
        IReadOnlyDictionary<Guid, UserActivityCounts> counts)
    {
        var activity = counts.TryGetValue(dto.Id, out var value)
            ? value
            : new UserActivityCounts(0, 0);

        return new UserAdminDto
        {
            Id = dto.Id,
            FullName = dto.FullName,
            Email = dto.Email,
            Role = dto.Role,
            OrderCount = activity.OrderCount,
            CartItemCount = activity.CartItemCount,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            IsDeleted = dto.IsDeleted
        };
    }

    #endregion
}