using Contracts;
using Library.Domain.Events;
using MassTransit;

namespace Library.Infrastructure.Events.Publishers;

public class BookEventPublisher : EventPublisherBase
{
    public BookEventPublisher(IPublishEndpoint publisher) : base(publisher)
    {
        Register<BookCreatedEvent>((ev, ct)     => Publish(LibraryEventFactory.Create(ev.BookId, EntityType.Book, DomainActions.Created,  new() { ["AuthorId"] = ev.AuthorPartyId.ToString() }), ct));
        Register<BookUpdatedEvent>((ev, ct)     => Publish(LibraryEventFactory.Create(ev.BookId, EntityType.Book, DomainActions.Updated), ct));
        Register<BookDeletedEvent>((ev, ct)     => Publish(LibraryEventFactory.Create(ev.BookId, EntityType.Book, DomainActions.Deleted), ct));
        Register<BookCopyAddedEvent>((ev, ct)   => Publish(LibraryEventFactory.Create(ev.BookId, EntityType.Book, DomainActions.Added,    new() { ["CopyId"] = ev.CopyId.ToString() }), ct));
        Register<BookCopyRemovedEvent>((ev, ct) => Publish(LibraryEventFactory.Create(ev.BookId, EntityType.Book, DomainActions.Removed,  new() { ["CopyId"] = ev.CopyId.ToString() }), ct));
    }
}
