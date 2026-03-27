using Library.Domain;
using Library.Domain.Entities;

namespace Library.Application.Services;

internal static class RoleTypeParse
{
    public static RoleType ParseOrThrow(string? value)
    {
        if (!Enum.TryParse(value, ignoreCase: true, out RoleType role))
            throw new DomainException(
                $"Invalid role. Valid values: {string.Join(", ", Enum.GetNames<RoleType>())}");
        return role;
    }
}
