using Library.Domain.Events;
using Library.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Library.Infrastructure.Events.CacheInvalidators;

public class BookCacheInvalidator : CacheInvalidatorBase
{
    public BookCacheInvalidator(IMemoryCache cache) : base(cache)
    {
        RegisterCrudInvalidation<BookCreatedEvent, BookUpdatedEvent, BookDeletedEvent>(
            CacheKeys.AllBooks,
            ev => CacheKeys.Book(ev.BookId),
            ev => CacheKeys.Book(ev.BookId));

        Register<BookCopyAddedEvent>((ev, _) =>
        {
            InvalidateListAndEntity(CacheKeys.AllBooks, CacheKeys.Book(ev.BookId));
            return Task.CompletedTask;
        });

        Register<BookCopyRemovedEvent>((ev, _) =>
        {
            InvalidateListAndEntity(CacheKeys.AllBooks, CacheKeys.Book(ev.BookId));
            return Task.CompletedTask;
        });
    }
}
