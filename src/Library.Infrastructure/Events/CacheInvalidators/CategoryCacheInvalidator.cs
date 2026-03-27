using Library.Domain.Events;
using Library.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Library.Infrastructure.Events.CacheInvalidators;

public class CategoryCacheInvalidator : CacheInvalidatorBase
{
    public CategoryCacheInvalidator(IMemoryCache cache) : base(cache)
    {
        RegisterCrudInvalidation<CategoryCreatedEvent, CategoryUpdatedEvent, CategoryDeletedEvent>(
            CacheKeys.AllCategories,
            ev => CacheKeys.Category(ev.CategoryId),
            ev => CacheKeys.Category(ev.CategoryId));
    }
}
