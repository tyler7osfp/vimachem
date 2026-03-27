using Library.Domain.Events;

namespace Library.Infrastructure.Events
{
    public interface IAfterCommitDomainEventHandler
    {
        IReadOnlyCollection<Type> HandledEventTypes { get; }

        Task HandleAsync(IDomainEvent domainEvent, CancellationToken ct = default);
    }
}

