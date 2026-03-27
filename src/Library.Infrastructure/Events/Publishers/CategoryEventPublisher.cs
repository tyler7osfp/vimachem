using Contracts;
using Library.Domain.Events;
using MassTransit;

namespace Library.Infrastructure.Events.Publishers;

public class CategoryEventPublisher : EventPublisherBase
{
    public CategoryEventPublisher(IPublishEndpoint publisher) : base(publisher)
    {
        Register<CategoryCreatedEvent>((ev, ct) => Publish(LibraryEventFactory.Create(ev.CategoryId, EntityType.Category, DomainActions.Created), ct));
        Register<CategoryUpdatedEvent>((ev, ct) => Publish(LibraryEventFactory.Create(ev.CategoryId, EntityType.Category, DomainActions.Updated), ct));
        Register<CategoryDeletedEvent>((ev, ct) => Publish(LibraryEventFactory.Create(ev.CategoryId, EntityType.Category, DomainActions.Deleted), ct));
    }
}
