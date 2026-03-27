using Library.Application.Dtos;
using Library.Application.Mapping;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application.Services;

public class CategoryCommandService(
    [FromKeyedServices(RepositoryKeys.Db)] ICategoryRepository categories,
    IUnitOfWork uow) : ICategoryCommandService
{
    public async Task<CategoryDto> CreateAsync(string name, CancellationToken ct = default)
    {
        var cat = Category.Create(name);
        await categories.AddAsync(cat, ct);
        await uow.SaveChangesAsync(ct);
        return CategoryDtoMapper.ToDto(cat);
    }

    public async Task<CategoryDto?> UpdateAsync(Guid id, string name, CancellationToken ct = default)
    {
        var cat = await categories.GetByIdAsync(id, ct);
        if (cat is null) return null;

        cat.Update(name);
        await uow.SaveChangesAsync(ct);
        return CategoryDtoMapper.ToDto(cat);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var cat = await categories.GetByIdAsync(id, ct);
        if (cat is null) return false;

        cat.MarkDeleted();
        categories.Delete(cat);
        await uow.SaveChangesAsync(ct);
        return true;
    }
}
