using Library.Application.Dtos;
using Library.Domain.Entities;

namespace Library.Application.Mapping;

internal static class PartyDtoMapper
{
    public static PartyListItemDto ToListItem(Party p)
        => new(
            p.Id,
            p.Name,
            p.Email,
            p.Roles.Select(r => r.Role.ToString()).ToList());

    public static PartyDetailDto ToDetail(
        Party party,
        IReadOnlyList<Book>? authoredBooks,
        IReadOnlyList<Borrowing>? activeBorrowings,
        IReadOnlyList<Borrowing>? pastBorrowings)
    {
        IReadOnlyList<PartyAuthoredBookDto>? authored = authoredBooks?.Select(b => new PartyAuthoredBookDto(
            b.Id,
            b.Title,
            b.Category?.Name ?? "",
            b.TotalCopies,
            b.AvailableCopies)).ToList();

        IReadOnlyList<PartyBorrowingRowDto>? active = activeBorrowings?.Select(MapBorrowingRow).ToList();
        IReadOnlyList<PartyBorrowingRowDto>? past = pastBorrowings?.Select(MapBorrowingRow).ToList();

        return new PartyDetailDto(
            party.Id,
            party.Name,
            party.Email,
            party.Roles.Select(r => r.Role.ToString()).ToList(),
            authored,
            active,
            past);
    }

    private static PartyBorrowingRowDto MapBorrowingRow(Borrowing b)
        => new(b.Id, b.BookCopy.BookId, b.BookCopy.Book.Title, b.BookCopyId, b.BorrowedAt, b.ReturnedAt);
}
