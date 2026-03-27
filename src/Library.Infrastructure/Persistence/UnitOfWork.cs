using Library.Domain.Interfaces;
using Library.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence
{
    public class UnitOfWork(LibraryDbContext dbContext, IDomainEventDispatcher dispatcher) : IUnitOfWork
    {
        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            var domainEvents = dbContext.ChangeTracker
                .Entries<Entity>()
                .SelectMany(e => e.Entity.PopDomainEvents())
                .ToList();

            foreach (var evt in domainEvents)
                await dispatcher.DispatchBeforeCommitAsync(evt, ct);

            await dbContext.SaveChangesAsync(ct);

            foreach (var evt in domainEvents)
                await dispatcher.DispatchAfterCommitAsync(evt, ct);
        }
    }
}
