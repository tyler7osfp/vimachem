using FluentAssertions;
using Library.Domain.Events;
using Library.Infrastructure.Caching;
using Library.Infrastructure.Events.CacheInvalidators;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace Library.Tests;

public class CacheInvalidatorTests
{
    private static IMemoryCache MakeCache() => Substitute.For<IMemoryCache>();

    [Fact]
    public async Task BookCacheInvalidator_BookCreated_RemovesAllBooksKey()
    {
        var cache = MakeCache();
        var sut = new BookCacheInvalidator(cache);

        await sut.HandleAsync(new BookCreatedEvent(Guid.NewGuid(), Guid.NewGuid()));

        cache.Received(1).Remove(CacheKeys.AllBooks);
    }

    [Fact]
    public async Task BookCacheInvalidator_BookUpdated_RemovesListAndEntityKeys()
    {
        var cache = MakeCache();
        var bookId = Guid.NewGuid();
        var sut = new BookCacheInvalidator(cache);

        await sut.HandleAsync(new BookUpdatedEvent(bookId));

        cache.Received(1).Remove(CacheKeys.AllBooks);
        cache.Received(1).Remove(CacheKeys.Book(bookId));
    }

    [Fact]
    public async Task BookCacheInvalidator_BookDeleted_RemovesListAndEntityKeys()
    {
        var cache = MakeCache();
        var bookId = Guid.NewGuid();
        var sut = new BookCacheInvalidator(cache);

        await sut.HandleAsync(new BookDeletedEvent(bookId));

        cache.Received(1).Remove(CacheKeys.AllBooks);
        cache.Received(1).Remove(CacheKeys.Book(bookId));
    }

    [Fact]
    public async Task BookCacheInvalidator_UnrelatedEvent_DoesNotTouchCache()
    {
        var cache = MakeCache();
        var sut = new BookCacheInvalidator(cache);

        await sut.HandleAsync(new PartyCreatedEvent(Guid.NewGuid()));

        cache.DidNotReceive().Remove(Arg.Any<object>());
    }

    [Fact]
    public async Task CategoryCacheInvalidator_CategoryUpdated_RemovesListAndEntityKeys()
    {
        var cache = MakeCache();
        var id = Guid.NewGuid();
        var sut = new CategoryCacheInvalidator(cache);

        await sut.HandleAsync(new CategoryUpdatedEvent(id));

        cache.Received(1).Remove(CacheKeys.AllCategories);
        cache.Received(1).Remove(CacheKeys.Category(id));
    }

    [Fact]
    public async Task CategoryCacheInvalidator_CategoryDeleted_RemovesListAndEntityKeys()
    {
        var cache = MakeCache();
        var id = Guid.NewGuid();
        var sut = new CategoryCacheInvalidator(cache);

        await sut.HandleAsync(new CategoryDeletedEvent(id));

        cache.Received(1).Remove(CacheKeys.AllCategories);
        cache.Received(1).Remove(CacheKeys.Category(id));
    }

    [Fact]
    public async Task PartyCacheInvalidator_PartyRoleAdded_RemovesListAndEntityKeys()
    {
        var cache = MakeCache();
        var id = Guid.NewGuid();
        var sut = new PartyCacheInvalidator(cache);

        await sut.HandleAsync(new PartyRoleAddedEvent(id, "Author"));

        cache.Received(1).Remove(CacheKeys.AllParties);
        cache.Received(1).Remove(CacheKeys.Party(id));
    }

    [Fact]
    public async Task BorrowingCacheInvalidator_BorrowingCreated_RemovesBookKeys()
    {
        var cache = MakeCache();
        var bookId = Guid.NewGuid();
        var sut = new BorrowingCacheInvalidator(cache);

        await sut.HandleAsync(new BorrowingCreatedEvent(Guid.NewGuid(), bookId, Guid.NewGuid(), Guid.NewGuid()));

        cache.Received(1).Remove(CacheKeys.AllBooks);
        cache.Received(1).Remove(CacheKeys.Book(bookId));
    }

    [Fact]
    public async Task BorrowingCacheInvalidator_BorrowingReturned_RemovesBookKeys()
    {
        var cache = MakeCache();
        var bookId = Guid.NewGuid();
        var sut = new BorrowingCacheInvalidator(cache);

        await sut.HandleAsync(new BorrowingReturnedEvent(Guid.NewGuid(), bookId, Guid.NewGuid(), Guid.NewGuid()));

        cache.Received(1).Remove(CacheKeys.AllBooks);
        cache.Received(1).Remove(CacheKeys.Book(bookId));
    }
}
