using EventService.Models;

namespace EventService.Repos;

public interface IEventWriteRepository
{
    Task InsertAsync(EventDocument doc, CancellationToken ct = default);
}
