using Library.Domain.Events;

namespace Library.Infrastructure.Events;

internal sealed class DomainEventHandlerRegistry
{
    private readonly Dictionary<Type, Func<IDomainEvent, CancellationToken, Task>> _handlers = new();

    public void Register<TEvent>(Func<TEvent, CancellationToken, Task> handler) where TEvent : IDomainEvent
        => _handlers[typeof(TEvent)] = (evt, ct) => handler((TEvent)evt, ct);

    public IReadOnlyCollection<Type> RegisteredEventTypes => _handlers.Keys.ToList();

    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default)
        => _handlers.TryGetValue(domainEvent.GetType(), out var handler)
            ? handler(domainEvent, ct)
            : Task.CompletedTask;
}
