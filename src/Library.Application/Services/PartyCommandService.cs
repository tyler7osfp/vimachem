using Library.Application.Dtos;
using Library.Application.Mapping;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application.Services;

public class PartyCommandService(
    [FromKeyedServices(RepositoryKeys.Db)] IPartyRepository parties,
    [FromKeyedServices(RepositoryKeys.Db)] IBookRepository books,
    IBorrowingRepository borrowings,
    IUnitOfWork uow) : IPartyCommandService
{
    public async Task<PartyListItemDto> CreateAsync(string name, string email, string initialRole, CancellationToken ct = default)
    {
        var roleType = RoleTypeParse.ParseOrThrow(initialRole);
        var party = Party.Create(name, email, roleType);
        await parties.AddAsync(party, ct);
        await uow.SaveChangesAsync(ct);

        var reloaded = await parties.GetByIdAsync(party.Id, ct) ?? party;
        return PartyDtoMapper.ToListItem(reloaded);
    }

    public async Task<PartyListItemDto?> UpdateAsync(Guid id, string name, string email, CancellationToken ct = default)
    {
        var party = await parties.GetByIdAsync(id, ct);
        if (party is null) return null;

        party.Update(name, email);
        await uow.SaveChangesAsync(ct);

        var reloaded = await parties.GetByIdAsync(id, ct) ?? party;
        return PartyDtoMapper.ToListItem(reloaded);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var party = await parties.GetByIdAsync(id, ct);
        if (party is null) return false;

        if (await books.HasBooksAsAuthorAsync(id, ct))
            throw new DomainException("Cannot delete a party that is the author of one or more books.");

        if (await borrowings.HasActiveBorrowingsForCustomerAsync(id, ct))
            throw new DomainException("Cannot delete a party that has active borrowings.");

        party.MarkDeleted();
        parties.Delete(party);
        await uow.SaveChangesAsync(ct);
        return true;
    }
}
