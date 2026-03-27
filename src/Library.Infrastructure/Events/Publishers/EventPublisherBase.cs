using Contracts;
using Library.Domain.Events;
using Library.Infrastructure.Events;
using MassTransit;

namespace Library.Infrastructure.Events.Publishers;

public abstract class EventPublisherBase : IBeforeCommitDomainEventHandler
{
    private readonly IPublishEndpoint _publisher;
    private readonly DomainEventHandlerRegistry _registry = new();

    protected EventPublisherBase(IPublishEndpoint publisher)
    {
        _publisher = publisher;
    }

    protected void Register<TEvent>(Func<TEvent, CancellationToken, Task> handler) where TEvent : IDomainEvent
        => _registry.Register(handler);

    public IReadOnlyCollection<Type> HandledEventTypes => _registry.RegisteredEventTypes;

    public Task HandleAsync(IDomainEvent domainEvent, CancellationToken ct = default)
        => _registry.DispatchAsync(domainEvent, ct);

    protected Task Publish(LibraryEvent evt, CancellationToken ct = default)
        => _publisher.Publish(evt, ct);
}
