using Library.Application.Dtos;
using Library.Application.Extensions;
using Library.Application.Mapping;
using Library.Domain;
using Library.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application.Services;

public class BookInventoryService(
    [FromKeyedServices(RepositoryKeys.Db)] IBookRepository books,
    IUnitOfWork uow) : IBookInventoryService
{
    public async Task<IReadOnlyList<BookCopyDto>> AddCopiesAsync(Guid bookId, int count, CancellationToken ct = default)
    {
        var book = (await books.GetByIdAsync(bookId, ct)).OrDomainException("Book not found.");

        var copies = book.AddCopies(count);
        foreach (var copy in copies)
            books.TrackNewCopy(copy);
        await uow.SaveChangesAsync(ct);
        return copies.Select(BookDtoMapper.ToCopyDto).ToList();
    }

    public async Task<bool> RemoveCopyAsync(Guid bookId, Guid copyId, CancellationToken ct = default)
    {
        var book = await books.GetByIdAsync(bookId, ct);
        if (book is null) return false;

        book.RemoveCopy(copyId);
        await uow.SaveChangesAsync(ct);
        return true;
    }
}
