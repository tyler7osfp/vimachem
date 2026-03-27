using Library.Application.Dtos;
using Library.Application.Extensions;
using Library.Application.Mapping;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application.Services;

public class BorrowingCommandService(
    IBorrowingRepository borrowings,
    [FromKeyedServices(RepositoryKeys.Db)] IBookRepository books,
    [FromKeyedServices(RepositoryKeys.Db)] IPartyRepository parties,
    IUnitOfWork uow) : IBorrowingCommandService
{
    public async Task<BorrowingDto> BorrowAsync(Guid bookId, Guid customerPartyId, CancellationToken ct = default)
    {
        var customer = (await parties.GetByIdAsync(customerPartyId, ct)).OrDomainException("Customer party not found.");
        var book = (await books.GetByIdAsync(bookId, ct)).OrDomainException("Book not found.");

        var copy = book.BorrowCopy(customer); // domain enforces Customer role
        var borrowing = Borrowing.Start(bookId, copy.Id, customerPartyId);

        await borrowings.AddAsync(borrowing, ct);
        await uow.SaveChangesAsync(ct);
        return BorrowingDtoMapper.ToDto(borrowing);
    }

    public async Task<BorrowingDto?> ReturnAsync(Guid borrowingId, CancellationToken ct = default)
    {
        var borrowing = await borrowings.GetByIdAsync(borrowingId, ct);
        if (borrowing is null) return null;

        var book = (await books.GetByIdAsync(borrowing.BookId, ct)).OrDomainException("Associated book not found.");

        book.ReturnCopy(borrowing.BookCopyId);
        borrowing.CompleteReturn();

        await uow.SaveChangesAsync(ct);
        return BorrowingDtoMapper.ToDto(borrowing);
    }
}
