using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Library.Infrastructure.Caching;

public abstract class CachingCrudRepositoryBase<TEntity>(IMemoryCache cache, IOptions<CacheOptions> options) where TEntity : class
{
    protected IMemoryCache Cache { get; } = cache;

    private TimeSpan CacheTtl => options.Value.Ttl;

    protected abstract object ListCacheKey { get; }

    protected abstract object EntityCacheKey(Guid id);

    protected Task<IEnumerable<TEntity>> GetAllCachedAsync(
        Func<CancellationToken, Task<IEnumerable<TEntity>>> load,
        CancellationToken ct = default)
        => CachingRepositoryHelper.GetAllAsync(Cache, ListCacheKey, load, CacheTtl, ct);

    protected Task<TEntity?> GetByIdCachedAsync(
        Guid id,
        Func<CancellationToken, Task<TEntity?>> load,
        CancellationToken ct = default)
        => CachingRepositoryHelper.GetByIdAsync(Cache, EntityCacheKey(id), load, CacheTtl, ct);

    protected void InvalidateListOnly()
        => CachingRepositoryHelper.InvalidateList(Cache, ListCacheKey);

    protected void InvalidateListAndEntity(Guid id)
        => CachingRepositoryHelper.InvalidateListAndItem(Cache, ListCacheKey, EntityCacheKey(id));
}
