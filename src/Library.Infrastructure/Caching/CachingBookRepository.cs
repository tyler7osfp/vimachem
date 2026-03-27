using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Library.Infrastructure.Caching;

public class CachingBookRepository([FromKeyedServices(RepositoryKeys.Db)] IBookRepository inner, IMemoryCache cache, IOptions<CacheOptions> options)
    : CachingCrudRepositoryBase<Book>(cache, options), IBookRepository
{
    protected override object ListCacheKey => CacheKeys.AllBooks;
    protected override object EntityCacheKey(Guid id) => CacheKeys.Book(id);

    public Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default)
        => GetAllCachedAsync(inner.GetAllAsync, ct);

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => GetByIdCachedAsync(id, c => inner.GetByIdAsync(id, c), ct);

    public Task<Book?> GetByTitleAsync(string title, CancellationToken ct = default)
        => inner.GetByTitleAsync(title, ct);

    public Task<IEnumerable<Book>> GetByAuthorPartyIdAsync(Guid authorPartyId, CancellationToken ct = default)
        => inner.GetByAuthorPartyIdAsync(authorPartyId, ct);

    public Task AddAsync(Book entity, CancellationToken ct = default)
    {
        InvalidateListOnly();
        return inner.AddAsync(entity, ct);
    }

    public void Update(Book entity)
    {
        InvalidateListAndEntity(entity.Id);
        inner.Update(entity);
    }

    public void Delete(Book entity)
    {
        InvalidateListAndEntity(entity.Id);
        inner.Delete(entity);
    }

    public Task<bool> HasBooksAsAuthorAsync(Guid authorPartyId, CancellationToken ct = default)
        => inner.HasBooksAsAuthorAsync(authorPartyId, ct);

    public void TrackNewCopy(BookCopy copy) => inner.TrackNewCopy(copy);
}
