using Contracts;
using FluentAssertions;
using Library.Domain.Events;
using Library.Infrastructure.Events.Publishers;
using MassTransit;
using NSubstitute;

namespace Library.Tests;

public class EventPublisherTests
{
    private static IPublishEndpoint MockEndpoint() => Substitute.For<IPublishEndpoint>();

    [Fact]
    public async Task BookEventPublisher_BookCreated_PublishesWithCorrectEntityTypeAndAction()
    {
        var endpoint = MockEndpoint();
        var sut = new BookEventPublisher(endpoint);
        var ev = new BookCreatedEvent(Guid.NewGuid(), Guid.NewGuid());

        await sut.HandleAsync(ev);

        await endpoint.Received(1).Publish(
            Arg.Is<LibraryEvent>(e => e.EntityId == ev.BookId
                && e.EntityType == EntityType.Book
                && e.Action == "Created"
                && e.Metadata["AuthorId"] == ev.AuthorPartyId.ToString()),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BookEventPublisher_BookDeleted_PublishesDeletedAction()
    {
        var endpoint = MockEndpoint();
        var sut = new BookEventPublisher(endpoint);
        var ev = new BookDeletedEvent(Guid.NewGuid());

        await sut.HandleAsync(ev);

        await endpoint.Received(1).Publish(
            Arg.Is<LibraryEvent>(e => e.EntityId == ev.BookId && e.Action == "Deleted"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BookEventPublisher_CopyAdded_IncludesCopyIdInMetadata()
    {
        var endpoint = MockEndpoint();
        var sut = new BookEventPublisher(endpoint);
        var ev = new BookCopyAddedEvent(Guid.NewGuid(), Guid.NewGuid());

        await sut.HandleAsync(ev);

        await endpoint.Received(1).Publish(
            Arg.Is<LibraryEvent>(e => e.Metadata["CopyId"] == ev.CopyId.ToString()),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BookEventPublisher_UnrelatedEvent_DoesNotPublish()
    {
        var endpoint = MockEndpoint();
        var sut = new BookEventPublisher(endpoint);

        await sut.HandleAsync(new PartyCreatedEvent(Guid.NewGuid()));

        await endpoint.DidNotReceive().Publish(Arg.Any<LibraryEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CategoryEventPublisher_CategoryCreated_PublishesWithCategoryEntityType()
    {
        var endpoint = MockEndpoint();
        var sut = new CategoryEventPublisher(endpoint);
        var ev = new CategoryCreatedEvent(Guid.NewGuid());

        await sut.HandleAsync(ev);

        await endpoint.Received(1).Publish(
            Arg.Is<LibraryEvent>(e => e.EntityId == ev.CategoryId
                && e.EntityType == EntityType.Category
                && e.Action == "Created"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PartyEventPublisher_RoleAdded_IncludesRoleInMetadata()
    {
        var endpoint = MockEndpoint();
        var sut = new PartyEventPublisher(endpoint);
        var ev = new PartyRoleAddedEvent(Guid.NewGuid(), "Author");

        await sut.HandleAsync(ev);

        await endpoint.Received(1).Publish(
            Arg.Is<LibraryEvent>(e => e.Metadata["Role"] == "Author"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BorrowingEventPublisher_BorrowingCreated_PublishesBorrowedAction()
    {
        var endpoint = MockEndpoint();
        var sut = new BorrowingEventPublisher(endpoint);
        var ev = new BorrowingCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        await sut.HandleAsync(ev);

        await endpoint.Received(1).Publish(
            Arg.Is<LibraryEvent>(e => e.EntityId == ev.BorrowingId
                && e.EntityType == EntityType.Borrowing
                && e.Action == "Borrowed"
                && e.Metadata["BookId"] == ev.BookId.ToString()
                && e.Metadata["CustomerPartyId"] == ev.CustomerPartyId.ToString()),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BorrowingEventPublisher_BorrowingReturned_PublishesReturnedAction()
    {
        var endpoint = MockEndpoint();
        var sut = new BorrowingEventPublisher(endpoint);
        var ev = new BorrowingReturnedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        await sut.HandleAsync(ev);

        await endpoint.Received(1).Publish(
            Arg.Is<LibraryEvent>(e => e.Action == "Returned"),
            Arg.Any<CancellationToken>());
    }
}
