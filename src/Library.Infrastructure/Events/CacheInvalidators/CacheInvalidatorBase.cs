using Library.Domain.Events;
using Library.Infrastructure.Events;
using Microsoft.Extensions.Caching.Memory;

namespace Library.Infrastructure.Events.CacheInvalidators;

public abstract class CacheInvalidatorBase(IMemoryCache cache) : IAfterCommitDomainEventHandler
{
    private readonly DomainEventHandlerRegistry _registry = new();

    protected void Register<TEvent>(Func<TEvent, CancellationToken, Task> handler) where TEvent : IDomainEvent
        => _registry.Register(handler);

    public IReadOnlyCollection<Type> HandledEventTypes => _registry.RegisteredEventTypes;

    public Task HandleAsync(IDomainEvent domainEvent, CancellationToken ct = default)
        => _registry.DispatchAsync(domainEvent, ct);

    protected void InvalidateList(string listKey)
        => cache.Remove(listKey);

    protected void InvalidateListAndEntity(string listKey, string entityKey)
    {
        cache.Remove(listKey);
        cache.Remove(entityKey);
    }

    protected void RegisterCrudInvalidation<TCreated, TUpdated, TDeleted>(
        string listKey,
        Func<TUpdated, string> updatedEntityKey,
        Func<TDeleted, string> deletedEntityKey)
        where TCreated : IDomainEvent
        where TUpdated : IDomainEvent
        where TDeleted : IDomainEvent
    {
        Register<TCreated>((_, _) => { InvalidateList(listKey); return Task.CompletedTask; });
        Register<TUpdated>((ev, _) => { InvalidateListAndEntity(listKey, updatedEntityKey(ev)); return Task.CompletedTask; });
        Register<TDeleted>((ev, _) => { InvalidateListAndEntity(listKey, deletedEntityKey(ev)); return Task.CompletedTask; });
    }
}
