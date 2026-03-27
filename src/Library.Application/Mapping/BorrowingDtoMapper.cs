using Library.Application.Dtos;
using Library.Domain.Entities;

namespace Library.Application.Mapping;

internal static class BorrowingDtoMapper
{
    public static BorrowingDto ToDto(Borrowing b)
        => new(b.Id, b.BookCopyId, b.CustomerPartyId, b.BorrowedAt, b.ReturnedAt);

    public static ActiveBorrowingListItemDto ToActiveListItem(Borrowing b)
        => new(b.Id, b.BookCopy.BookId, b.BookCopy.Book.Title, b.BookCopyId, b.CustomerPartyId, b.Customer.Name, b.BorrowedAt);

    public static BookBorrowingListItemDto ToBookBorrowingListItem(Borrowing b)
        => new(b.Id, b.BookCopyId, b.CustomerPartyId, b.Customer.Name, b.BorrowedAt, b.ReturnedAt);
}
