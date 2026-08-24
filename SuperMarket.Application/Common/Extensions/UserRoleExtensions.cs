using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.Common.Extensions;

public static class UserRoleExtensions
{
    public static string ToRoleName(this UserRole role)
    {
        return role.ToString();
    }

    public static bool TryToUserRole(
        this string value,
        out UserRole role)
    {
        return Enum.TryParse(
                   value,
                   true,
                   out role)
               && role.IsValid();
    }

    public static bool IsValid(this UserRole role)
    {
        return role != UserRole.None &&
               Enum.IsDefined(typeof(UserRole), role);
    }
}