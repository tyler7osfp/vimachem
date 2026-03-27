namespace Library.Application.Dtos;

public sealed record BorrowingDto(
    Guid Id,
    Guid BookCopyId,
    Guid CustomerPartyId,
    DateTime BorrowedAt,
    DateTime? ReturnedAt);

public sealed record ActiveBorrowingListItemDto(
    Guid Id,
    Guid BookId,
    string BookTitle,
    Guid BookCopyId,
    Guid CustomerPartyId,
    string CustomerName,
    DateTime BorrowedAt);

public sealed record BookBorrowingListItemDto(
    Guid Id,
    Guid BookCopyId,
    Guid CustomerPartyId,
    string CustomerName,
    DateTime BorrowedAt,
    DateTime? ReturnedAt);

public sealed record BorrowingVisibilityCustomerDto(Guid PartyId, string Name);

public sealed record BorrowingVisibilityItemDto(
    Guid BookId,
    string Title,
    IReadOnlyList<BorrowingVisibilityCustomerDto> CurrentBorrowers);
