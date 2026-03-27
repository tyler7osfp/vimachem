using Library.Application.Dtos;

namespace Library.Application.Services;

public interface IBookCommandService
{
    Task<BookListItemDto> CreateAsync(string title, Guid categoryId, Guid authorPartyId, int copies, CancellationToken ct = default);
    Task<BookListItemDto?> UpdateAsync(Guid id, string title, Guid categoryId, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
