

using SuperMarket.Application.DTOs.Users;
using SuperMarket.Web.Areas.Admin.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperMarket.Web.Mapping
{
    public static class UserMappings
    {
        public static UserListItemViewModel ToListItemViewModel(UserAdminDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new UserListItemViewModel
            {
                Id = dto.Id,
                FullName = dto.FullName ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                Role = dto.Role.ToString(),
                OrderCount = dto.OrderCount,
                CartItemCount = dto.CartItemCount,
                CreatedAt = dto.CreatedAt,
                IsDeleted = dto.IsDeleted
            };
        }

        public static UserDetailsViewModel ToDetailsViewModel(UserAdminDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new UserDetailsViewModel
            {
                Id = dto.Id,
                FullName = dto.FullName ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                Role = dto.Role.ToString(),
                OrderCount = dto.OrderCount,
                CartItemCount = dto.CartItemCount,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                IsDeleted = dto.IsDeleted
            };
        }

        public static IReadOnlyList<UserListItemViewModel> ToListItemViewModels(IEnumerable<UserAdminDto> dtos)
        {
            if (dtos == null) return Array.Empty<UserListItemViewModel>();
            return dtos.Select(ToListItemViewModel).ToList().AsReadOnly();
        }

        public static UserAdminDto ToAdminDto(UserCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new UserAdminDto
            {
                Id = Guid.NewGuid(),
                FullName = $"{dto.FirstName} {dto.LastName}",
                Email = dto.Email,
                Role = dto.Role,
                OrderCount = 0,
                CartItemCount = 0,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = null,
                IsDeleted = false
            };
        }

        public static UserAdminDto ToAdminDto(UserUpdateDto dto, UserAdminDto existing)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (existing == null)
                throw new ArgumentNullException(nameof(existing));

            return new UserAdminDto
            {
                Id = existing.Id,
                FullName = $"{dto.FirstName} {dto.LastName}",
                Email = dto.Email,
                Role = dto.Role,
                OrderCount = existing.OrderCount,
                CartItemCount = existing.CartItemCount,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTimeOffset.UtcNow,
                IsDeleted = existing.IsDeleted
            };
        }
    }
}

