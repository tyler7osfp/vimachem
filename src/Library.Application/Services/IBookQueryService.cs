using Library.Application.Dtos;

namespace Library.Application.Services;

public interface IBookQueryService
{
    Task<IEnumerable<BookListItemDto>> GetAllAsync(CancellationToken ct = default);
    Task<BookListItemDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BookListItemDto?> GetByTitleAsync(string title, CancellationToken ct = default);
    Task<BookAvailabilityDto?> GetAvailabilityByIdAsync(Guid id, CancellationToken ct = default);
    Task<BookAvailabilityDto?> GetAvailabilityByTitleAsync(string title, CancellationToken ct = default);
}
