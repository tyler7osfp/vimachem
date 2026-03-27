namespace EventService.Repos;

public interface IEventRepository : IEventReadRepository, IEventWriteRepository, IEventRetentionRepository
{
    void EnsureIndexes();
}
