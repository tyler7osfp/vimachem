using Library.Application.Dtos;
using Library.Application.Extensions;
using Library.Application.Mapping;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application.Services;

public class BookCommandService(
    [FromKeyedServices(RepositoryKeys.Db)] IBookRepository books,
    [FromKeyedServices(RepositoryKeys.Db)] IPartyRepository parties,
    [FromKeyedServices(RepositoryKeys.Db)] ICategoryRepository categories,
    IUnitOfWork uow) : IBookCommandService
{
    public async Task<BookListItemDto> CreateAsync(string title, Guid categoryId, Guid authorPartyId, int copies, CancellationToken ct = default)
    {
        var author = (await parties.GetByIdAsync(authorPartyId, ct)).OrDomainException("Author party not found.");
        var category = (await categories.GetByIdAsync(categoryId, ct)).OrDomainException("Category not found.");

        var book = Book.Create(title, author, categoryId, copies);
        await books.AddAsync(book, ct);
        await uow.SaveChangesAsync(ct);

        return BookDtoMapper.ToListItem(book, category.Name, author.Name);
    }

    public async Task<BookListItemDto?> UpdateAsync(Guid id, string title, Guid categoryId, CancellationToken ct = default)
    {
        var book = await books.GetByIdAsync(id, ct);
        if (book is null) return null;

        var category = (await categories.GetByIdAsync(categoryId, ct)).OrDomainException("Category not found.");

        book.Update(title, categoryId);
        await uow.SaveChangesAsync(ct);

        return BookDtoMapper.ToListItem(book, category.Name, book.Author?.Name ?? "");
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var book = await books.GetByIdAsync(id, ct);
        if (book is null) return false;

        book.MarkDeleted();
        books.Delete(book);
        await uow.SaveChangesAsync(ct);
        return true;
    }
}
