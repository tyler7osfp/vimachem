using Library.Domain.Events;
using Library.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Library.Infrastructure.Events.CacheInvalidators;

public class PartyCacheInvalidator : CacheInvalidatorBase
{
    public PartyCacheInvalidator(IMemoryCache cache) : base(cache)
    {
        RegisterCrudInvalidation<PartyCreatedEvent, PartyUpdatedEvent, PartyDeletedEvent>(
            CacheKeys.AllParties,
            ev => CacheKeys.Party(ev.PartyId),
            ev => CacheKeys.Party(ev.PartyId));

        Register<PartyRoleAddedEvent>((ev, _) =>
        {
            InvalidateListAndEntity(CacheKeys.AllParties, CacheKeys.Party(ev.PartyId));
            return Task.CompletedTask;
        });

        Register<PartyRoleRemovedEvent>((ev, _) =>
        {
            InvalidateListAndEntity(CacheKeys.AllParties, CacheKeys.Party(ev.PartyId));
            return Task.CompletedTask;
        });
    }
}
