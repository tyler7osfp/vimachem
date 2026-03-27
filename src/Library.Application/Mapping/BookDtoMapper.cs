using Library.Application.Dtos;
using Library.Domain.Entities;

namespace Library.Application.Mapping;

internal static class BookDtoMapper
{
    public static BookListItemDto ToListItem(Book b)
        => new(
            b.Id,
            b.Title,
            b.Category?.Name ?? string.Empty,
            new BookAuthorDto(b.AuthorPartyId, b.Author?.Name ?? ""),
            b.TotalCopies,
            b.AvailableCopies,
            b.IsAvailable(),
            GetActiveCopies(b));

    public static BookListItemDto ToListItem(Book b, string categoryName, string authorName)
        => new(
            b.Id,
            b.Title,
            categoryName,
            new BookAuthorDto(b.AuthorPartyId, authorName),
            b.TotalCopies,
            b.AvailableCopies,
            b.IsAvailable(),
            GetActiveCopies(b));

    public static BookAvailabilityDto ToAvailability(Book b)
        => new(
            b.Id,
            b.Title,
            new BookAuthorDto(b.AuthorPartyId, b.Author?.Name ?? ""),
            b.TotalCopies,
            b.AvailableCopies,
            b.IsAvailable(),
            GetActiveCopies(b));

    private static List<BookCopyDto> GetActiveCopies(Book b)
        => b.Copies.Where(c => !c.IsDeleted).Select(c => new BookCopyDto(c.Id, c.IsAvailable)).ToList();

    public static BookCopyDto ToCopyDto(BookCopy c)
        => new(c.Id, c.IsAvailable);
}
