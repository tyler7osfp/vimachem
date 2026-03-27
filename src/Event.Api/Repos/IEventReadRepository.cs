using Contracts;
using EventService.Models;

namespace EventService.Repos;

public interface IEventReadRepository
{
    Task<(IEnumerable<EventDocument> Items, long Total)> GetByEntityIdAsync(Guid entityId, int page, int pageSize, CancellationToken ct = default);
    Task<(IEnumerable<EventDocument> Items, long Total)> GetByEntityTypeAsync(EntityType entityType, int page, int pageSize, CancellationToken ct = default);
}
