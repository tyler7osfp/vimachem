using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Library.Infrastructure.Caching;

public class CachingPartyRepository([FromKeyedServices(RepositoryKeys.Db)] IPartyRepository inner, IMemoryCache cache, IOptions<CacheOptions> options)
    : CachingCrudRepositoryBase<Party>(cache, options), IPartyRepository
{
    protected override object ListCacheKey => CacheKeys.AllParties;

    protected override object EntityCacheKey(Guid id) => CacheKeys.Party(id);

    public Task<IEnumerable<Party>> GetAllAsync(CancellationToken ct = default)
        => GetAllCachedAsync(inner.GetAllAsync, ct);

    public Task<Party?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => GetByIdCachedAsync(id, c => inner.GetByIdAsync(id, c), ct);

    public Task AddAsync(Party entity, CancellationToken ct = default)
    {
        InvalidateListOnly();
        return inner.AddAsync(entity, ct);
    }

    public void Update(Party entity)
    {
        InvalidateListAndEntity(entity.Id);
        inner.Update(entity);
    }

    public void Delete(Party entity)
    {
        InvalidateListAndEntity(entity.Id);
        inner.Delete(entity);
    }

}
