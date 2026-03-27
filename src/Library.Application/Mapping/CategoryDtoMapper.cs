using Library.Application.Dtos;
using Library.Domain.Entities;

namespace Library.Application.Mapping;

internal static class CategoryDtoMapper
{
    public static CategoryDto ToDto(Category c) => new(c.Id, c.Name);
}
