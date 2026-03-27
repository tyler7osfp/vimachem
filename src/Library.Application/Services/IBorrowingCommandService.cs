using Library.Application.Dtos;

namespace Library.Application.Services;

public interface IBorrowingCommandService
{
    Task<BorrowingDto> BorrowAsync(Guid bookId, Guid customerPartyId, CancellationToken ct = default);
    Task<BorrowingDto?> ReturnAsync(Guid borrowingId, CancellationToken ct = default);
}
