using Library.Domain.Events;

namespace Library.Domain.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchBeforeCommitAsync(IDomainEvent domainEvent, CancellationToken ct = default);
        Task DispatchAfterCommitAsync(IDomainEvent domainEvent, CancellationToken ct = default);
    }
}
