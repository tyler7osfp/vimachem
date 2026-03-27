using Library.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/books/{bookId:guid}/copies")]
public class BookCopiesController(IBookInventoryService _inventory) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddCopies(Guid bookId, [FromQuery] int count = 1, CancellationToken ct = default)
    {
        var copies = await _inventory.AddCopiesAsync(bookId, count, ct);
        return Ok(copies);
    }

    [HttpDelete("{copyId:guid}")]
    public async Task<IActionResult> RemoveCopy(Guid bookId, Guid copyId, CancellationToken ct)
    {
        var removed = await _inventory.RemoveCopyAsync(bookId, copyId, ct);
        return removed ? NoContent() : NotFound();
    }
}
