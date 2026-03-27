using Library.Application.Dtos;

namespace Library.Application.Services;

public interface ICategoryCommandService
{
    Task<CategoryDto> CreateAsync(string name, CancellationToken ct = default);
    Task<CategoryDto?> UpdateAsync(Guid id, string name, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
