using Library.Domain.Entities;

namespace Library.Application.Extensions;

public static class EntityExtensions
{
    public static T OrDomainException<T>(this T? entity, string message) where T : class
        => entity ?? throw new DomainException(message);
}
