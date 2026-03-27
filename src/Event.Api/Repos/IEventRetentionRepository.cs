namespace EventService.Repos;

public interface IEventRetentionRepository
{
    Task DeleteOlderByCutoffDate(DateTime date, CancellationToken ct = default);
}
