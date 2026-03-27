using Library.Application.Requests;
using Library.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BorrowingsController(IBorrowingCommandService _commands, IBorrowingQueryService _queries) : LibraryControllerBase
{
    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken ct)
        => Ok(await _queries.GetActiveBorrowingsAsync(ct));

    [HttpGet("visibility")]
    public async Task<IActionResult> GetVisibility(CancellationToken ct)
        => Ok(await _queries.GetBorrowingVisibilityAsync(ct));

    [HttpGet("~/api/books/{bookId:guid}/borrowings")]
    public async Task<IActionResult> GetByBookId(Guid bookId, CancellationToken ct)
        => Ok(await _queries.GetByBookIdAsync(bookId, ct));

    [HttpPost]
    public async Task<IActionResult> Borrow([FromBody] CreateBorrowingRequest req, CancellationToken ct)
    {
        var borrowing = await _commands.BorrowAsync(req.BookId, req.CustomerPartyId, ct);
        return Ok(borrowing);
    }

    [HttpPut("{id:guid}/return")]
    public async Task<IActionResult> Return(Guid id, CancellationToken ct)
    {
        var borrowing = await _commands.ReturnAsync(id, ct);
        return OkOrNotFound(borrowing);
    }
}
