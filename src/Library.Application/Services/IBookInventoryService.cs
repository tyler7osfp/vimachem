using Library.Application.Dtos;

namespace Library.Application.Services;

public interface IBookInventoryService
{
    Task<IReadOnlyList<BookCopyDto>> AddCopiesAsync(Guid bookId, int count, CancellationToken ct = default);
    Task<bool> RemoveCopyAsync(Guid bookId, Guid copyId, CancellationToken ct = default);
}
