using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Library.Infrastructure.Caching;

public class CachingCategoryRepository([FromKeyedServices(RepositoryKeys.Db)] ICategoryRepository inner, IMemoryCache cache, IOptions<CacheOptions> options)
    : CachingCrudRepositoryBase<Category>(cache, options), ICategoryRepository
{
    protected override object ListCacheKey => CacheKeys.AllCategories;

    protected override object EntityCacheKey(Guid id) => CacheKeys.Category(id);

    public Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default)
        => GetAllCachedAsync(inner.GetAllAsync, ct);

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => GetByIdCachedAsync(id, c => inner.GetByIdAsync(id, c), ct);

    public Task AddAsync(Category entity, CancellationToken ct = default)
    {
        InvalidateListOnly();
        return inner.AddAsync(entity, ct);
    }

    public void Update(Category entity)
    {
        InvalidateListAndEntity(entity.Id);
        inner.Update(entity);
    }

    public void Delete(Category entity)
    {
        InvalidateListAndEntity(entity.Id);
        inner.Delete(entity);
    }
}
