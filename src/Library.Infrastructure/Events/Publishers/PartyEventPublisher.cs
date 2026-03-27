using Contracts;
using Library.Domain.Events;
using MassTransit;

namespace Library.Infrastructure.Events.Publishers;

public class PartyEventPublisher : EventPublisherBase
{
    public PartyEventPublisher(IPublishEndpoint publisher) : base(publisher)
    {
        Register<PartyCreatedEvent>((ev, ct)     => Publish(LibraryEventFactory.Create(ev.PartyId, EntityType.Party, DomainActions.Created), ct));
        Register<PartyUpdatedEvent>((ev, ct)     => Publish(LibraryEventFactory.Create(ev.PartyId, EntityType.Party, DomainActions.Updated), ct));
        Register<PartyDeletedEvent>((ev, ct)     => Publish(LibraryEventFactory.Create(ev.PartyId, EntityType.Party, DomainActions.Deleted), ct));
        Register<PartyRoleAddedEvent>((ev, ct)   => Publish(LibraryEventFactory.Create(ev.PartyId, EntityType.Party, DomainActions.Added,   new() { ["Role"] = ev.Role }), ct));
        Register<PartyRoleRemovedEvent>((ev, ct) => Publish(LibraryEventFactory.Create(ev.PartyId, EntityType.Party, DomainActions.Removed, new() { ["Role"] = ev.Role }), ct));
    }
}
