using Library.Application.Dtos;
using Library.Application.Mapping;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application.Services;

public class CategoryQueryService(
    [FromKeyedServices(RepositoryKeys.Cached)] ICategoryRepository categories)
    : QueryServiceBase<Category, CategoryDto>(categories), ICategoryQueryService
{
    protected override CategoryDto Map(Category c) => CategoryDtoMapper.ToDto(c);
}
