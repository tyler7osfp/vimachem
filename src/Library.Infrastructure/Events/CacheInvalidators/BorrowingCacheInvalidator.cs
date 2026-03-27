using Library.Domain.Events;
using Library.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Library.Infrastructure.Events.CacheInvalidators;

public class BorrowingCacheInvalidator : CacheInvalidatorBase
{
    public BorrowingCacheInvalidator(IMemoryCache cache) : base(cache)
    {
        Register<BorrowingCreatedEvent>((ev, _) =>
        {
            InvalidateListAndEntity(CacheKeys.AllBooks, CacheKeys.Book(ev.BookId));
            return Task.CompletedTask;
        });

        Register<BorrowingReturnedEvent>((ev, _) =>
        {
            InvalidateListAndEntity(CacheKeys.AllBooks, CacheKeys.Book(ev.BookId));
            return Task.CompletedTask;
        });
    }
}
