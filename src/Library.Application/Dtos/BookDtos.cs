namespace Library.Application.Dtos;

public sealed record BookCopyDto(Guid Id, bool IsAvailable);

public sealed record BookAuthorDto(Guid PartyId, string Name);

public sealed record BookListItemDto(
    Guid Id,
    string Title,
    string Category,
    BookAuthorDto Author,
    int TotalCopies,
    int AvailableCopies,
    bool IsAvailable,
    IReadOnlyList<BookCopyDto> Copies);

public sealed record BookAvailabilityDto(
    Guid Id,
    string Title,
    BookAuthorDto Author,
    int TotalCopies,
    int AvailableCopies,
    bool IsAvailable,
    IReadOnlyList<BookCopyDto> Copies);
