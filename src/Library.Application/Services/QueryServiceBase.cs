using Library.Domain.Interfaces;

namespace Library.Application.Services;

public abstract class QueryServiceBase<TEntity, TDto>(IRepository<TEntity> repo)
    where TEntity : class
{
    protected abstract TDto Map(TEntity entity);

    public async Task<IEnumerable<TDto>> GetAllAsync(CancellationToken ct = default)
        => (await repo.GetAllAsync(ct)).Select(Map);

    public async Task<TDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        return entity is null ? default : Map(entity);
    }
}
