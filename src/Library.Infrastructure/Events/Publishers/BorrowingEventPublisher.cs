using Contracts;
using Library.Domain.Events;
using MassTransit;

namespace Library.Infrastructure.Events.Publishers;

public class BorrowingEventPublisher : EventPublisherBase
{
    public BorrowingEventPublisher(IPublishEndpoint publisher) : base(publisher)
    {
        Register<BorrowingCreatedEvent>((ev, ct)  => Publish(LibraryEventFactory.Create(ev.BorrowingId, EntityType.Borrowing, DomainActions.Borrowed, BorrowingMeta(ev.BookId, ev.CopyId, ev.CustomerPartyId)), ct));
        Register<BorrowingReturnedEvent>((ev, ct) => Publish(LibraryEventFactory.Create(ev.BorrowingId, EntityType.Borrowing, DomainActions.Returned, BorrowingMeta(ev.BookId, ev.CopyId, ev.CustomerPartyId)), ct));
    }

    private static Dictionary<string, string> BorrowingMeta(Guid bookId, Guid copyId, Guid customerId)
        => new() { ["BookId"] = bookId.ToString(), ["CopyId"] = copyId.ToString(), ["CustomerPartyId"] = customerId.ToString() };
}
