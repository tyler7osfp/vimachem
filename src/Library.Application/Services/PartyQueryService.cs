using Library.Application.Dtos;
using Library.Application.Mapping;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application.Services;

public class PartyQueryService(
    [FromKeyedServices(RepositoryKeys.Cached)] IPartyRepository parties,
    [FromKeyedServices(RepositoryKeys.Db)] IBookRepository books,
    IBorrowingRepository borrowings)
    : QueryServiceBase<Party, PartyListItemDto>(parties), IPartyQueryService
{
    protected override PartyListItemDto Map(Party p) => PartyDtoMapper.ToListItem(p);

    public async Task<PartyDetailDto?> GetByIdAsync(
        Guid id,
        bool includeAuthoredBooks = false,
        bool includeActiveBorrowings = false,
        bool includePastBorrowings = false,
        CancellationToken ct = default)
    {
        var party = await parties.GetByIdAsync(id, ct);
        if (party is null) return null;

        IReadOnlyList<Book>? authored = null;
        if (includeAuthoredBooks)
            authored = (await books.GetByAuthorPartyIdAsync(id, ct)).ToList();

        IReadOnlyList<Borrowing>? active = null;
        if (includeActiveBorrowings)
            active = (await borrowings.GetActiveBorrowingsForCustomerAsync(id, ct)).ToList();

        IReadOnlyList<Borrowing>? past = null;
        if (includePastBorrowings)
            past = (await borrowings.GetPastBorrowingsForCustomerAsync(id, ct)).ToList();

        return PartyDtoMapper.ToDetail(party, authored, active, past);
    }
}
