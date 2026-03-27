using Library.Application.Dtos;

namespace Library.Application.Services;

public interface IPartyQueryService
{
    Task<IEnumerable<PartyListItemDto>> GetAllAsync(CancellationToken ct = default);
    Task<PartyDetailDto?> GetByIdAsync(
        Guid id,
        bool includeAuthoredBooks = false,
        bool includeActiveBorrowings = false,
        bool includePastBorrowings = false,
        CancellationToken ct = default);
}
