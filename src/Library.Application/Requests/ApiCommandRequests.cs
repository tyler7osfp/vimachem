namespace Library.Application.Requests;

public record CreateBookRequest(string Title, Guid CategoryId, Guid AuthorPartyId, int Copies);
public record UpdateBookRequest(string Title, Guid CategoryId);
public record CreatePartyRequest(string Name, string Email, string InitialRole);
public record UpdatePartyRequest(string Name, string Email);
public record CreateCategoryRequest(string Name);
public record UpdateCategoryRequest(string Name);
public record CreateBorrowingRequest(Guid BookId, Guid CustomerPartyId);
