using Library.Domain.Events;
using Library.Domain.Interfaces;

namespace Library.Infrastructure.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly Dictionary<Type, IBeforeCommitDomainEventHandler> _beforeByType;
        private readonly Dictionary<Type, IAfterCommitDomainEventHandler> _afterByType;

        public DomainEventDispatcher(
            IEnumerable<IBeforeCommitDomainEventHandler> beforeCommitHandlers,
            IEnumerable<IAfterCommitDomainEventHandler> afterCommitHandlers)
        {
            _beforeByType = BuildHandlerMap(beforeCommitHandlers, h => h.HandledEventTypes);
            _afterByType = BuildHandlerMap(afterCommitHandlers, h => h.HandledEventTypes);
        }

        private static Dictionary<Type, THandler> BuildHandlerMap<THandler>(
            IEnumerable<THandler> handlers,
            Func<THandler, IReadOnlyCollection<Type>> getTypes)
        {
            var map = new Dictionary<Type, THandler>();
            foreach (var h in handlers)
            {
                foreach (var t in getTypes(h))
                {
                    if (map.ContainsKey(t))
                        throw new InvalidOperationException($"Duplicate domain event handler for event type {t.FullName}.");
                    map[t] = h;
                }
            }
            return map;
        }

        public Task DispatchBeforeCommitAsync(IDomainEvent domainEvent, CancellationToken ct = default)
            => _beforeByType.TryGetValue(domainEvent.GetType(), out var handler)
                ? handler.HandleAsync(domainEvent, ct)
                : Task.CompletedTask;

        public Task DispatchAfterCommitAsync(IDomainEvent domainEvent, CancellationToken ct = default)
            => _afterByType.TryGetValue(domainEvent.GetType(), out var handler)
                ? handler.HandleAsync(domainEvent, ct)
                : Task.CompletedTask;
    }
}
