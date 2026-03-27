using Library.Application.Dtos;

namespace Library.Application.Services;

public interface ICategoryQueryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
