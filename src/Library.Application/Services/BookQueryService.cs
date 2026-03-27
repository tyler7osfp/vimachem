using Library.Application.Dtos;
using Library.Application.Mapping;
using Library.Domain;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application.Services;

public class BookQueryService(
    [FromKeyedServices(RepositoryKeys.Cached)] IBookRepository books)
    : QueryServiceBase<Book, BookListItemDto>(books), IBookQueryService
{
    protected override BookListItemDto Map(Book b) => BookDtoMapper.ToListItem(b);

    public async Task<BookListItemDto?> GetByTitleAsync(string title, CancellationToken ct = default)
    {
        var b = await books.GetByTitleAsync(title, ct);
        return b is null ? null : Map(b);
    }

    public async Task<BookAvailabilityDto?> GetAvailabilityByIdAsync(Guid id, CancellationToken ct = default)
    {
        var b = await books.GetByIdAsync(id, ct);
        return b is null ? null : BookDtoMapper.ToAvailability(b);
    }

    public async Task<BookAvailabilityDto?> GetAvailabilityByTitleAsync(string title, CancellationToken ct = default)
    {
        var b = await books.GetByTitleAsync(title, ct);
        return b is null ? null : BookDtoMapper.ToAvailability(b);
    }
}
