using Library.Application.Dtos;

namespace Library.Application.Services;

public interface IBorrowingQueryService
{
    Task<IEnumerable<ActiveBorrowingListItemDto>> GetActiveBorrowingsAsync(CancellationToken ct = default);
    Task<IEnumerable<BookBorrowingListItemDto>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default);
    Task<IEnumerable<BorrowingVisibilityItemDto>> GetBorrowingVisibilityAsync(CancellationToken ct = default);
}
