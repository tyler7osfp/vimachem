using Library.Application.Dtos;
using Library.Application.Mapping;
using Library.Domain.Interfaces;

namespace Library.Application.Services;

public class BorrowingQueryService(IBorrowingRepository borrowings) : IBorrowingQueryService
{
    public async Task<IEnumerable<ActiveBorrowingListItemDto>> GetActiveBorrowingsAsync(CancellationToken ct = default)
    {
        var list = await borrowings.GetActiveBorrowingsAsync(ct);
        return list.Select(BorrowingDtoMapper.ToActiveListItem);
    }

    public async Task<IEnumerable<BookBorrowingListItemDto>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default)
    {
        var list = await borrowings.GetByBookIdAsync(bookId, ct);
        return list.Select(BorrowingDtoMapper.ToBookBorrowingListItem);
    }

    public async Task<IEnumerable<BorrowingVisibilityItemDto>> GetBorrowingVisibilityAsync(CancellationToken ct = default)
    {
        var active = await borrowings.GetActiveBorrowingsAsync(ct);

        return active
            .GroupBy(b => new { b.BookId, Title = b.BookCopy.Book.Title })
            .OrderBy(g => g.Key.Title)
            .Select(g => new BorrowingVisibilityItemDto(
                g.Key.BookId,
                g.Key.Title,
                g.Select(b => new BorrowingVisibilityCustomerDto(b.CustomerPartyId, b.Customer.Name))
                    .Distinct()
                    .OrderBy(c => c.Name)
                    .ToList()))
            .ToList();
    }
}
