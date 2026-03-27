using Library.Application.Extensions;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application.Services;

public class PartyRoleCommandService(
    [FromKeyedServices(RepositoryKeys.Db)] IPartyRepository parties,
    [FromKeyedServices(RepositoryKeys.Db)] IBookRepository books,
    IBorrowingRepository borrowings,
    IUnitOfWork uow) : IPartyRoleCommandService
{
    public async Task AddRoleAsync(Guid id, string role, CancellationToken ct = default)
    {
        var roleType = RoleTypeParse.ParseOrThrow(role);
        var party = (await parties.GetByIdAsync(id, ct)).OrDomainException("Party not found.");

        party.AddRole(roleType);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<bool> RemoveRoleAsync(Guid id, string role, CancellationToken ct = default)
    {
        var party = await parties.GetByIdAsync(id, ct);
        if (party is null) return false;

        var roleType = RoleTypeParse.ParseOrThrow(role);

        if (roleType == RoleType.Author && await books.HasBooksAsAuthorAsync(id, ct))
            throw new DomainException("Cannot remove the Author role while the party is the author of one or more books.");

        if (roleType == RoleType.Customer && await borrowings.HasActiveBorrowingsForCustomerAsync(id, ct))
            throw new DomainException("Cannot remove the Customer role while the party has active borrowings.");

        party.RemoveRole(roleType);
        await uow.SaveChangesAsync(ct);
        return true;
    }
}
