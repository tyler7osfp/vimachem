using Library.Domain.Events;

namespace Library.Infrastructure.Events
{
    public interface IBeforeCommitDomainEventHandler
    {
        IReadOnlyCollection<Type> HandledEventTypes { get; }

        Task HandleAsync(IDomainEvent domainEvent, CancellationToken ct = default);
    }
}

