using System.Text.Json.Serialization;

namespace Library.Application.Dtos;

public sealed record PartyListItemDto(Guid Id, string Name, string Email, IReadOnlyList<string> Roles);

public sealed record PartyAuthoredBookDto(Guid Id, string Title, string CategoryName, int TotalCopies, int AvailableCopies);

public sealed record PartyBorrowingRowDto(
    Guid BorrowingId,
    Guid BookId,
    string BookTitle,
    Guid BookCopyId,
    DateTime BorrowedAt,
    DateTime? ReturnedAt);

public sealed record PartyDetailDto(
    Guid Id,
    string Name,
    string Email,
    IReadOnlyList<string> Roles,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyList<PartyAuthoredBookDto>? AuthoredBooks,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyList<PartyBorrowingRowDto>? ActiveBorrowings,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyList<PartyBorrowingRowDto>? PastBorrowings);
