using Library.Application.Dtos;
using Library.Application.Requests;
using Library.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(
    IBookQueryService _queries,
    IBookCommandService _commands) : LibraryControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _queries.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var book = await _queries.GetByIdAsync(id, ct);
        return OkOrNotFound(book);
    }

    [HttpGet("availability")]
    public async Task<IActionResult> CheckAvailability([FromQuery] Guid? id, [FromQuery] string? title, CancellationToken ct)
    {
        if (!id.HasValue && string.IsNullOrWhiteSpace(title))
            return BadRequest("Provide either 'id' or 'title'.");

        BookAvailabilityDto? book = id.HasValue
            ? await _queries.GetAvailabilityByIdAsync(id.Value, ct)
            : await _queries.GetAvailabilityByTitleAsync(title!, ct);

        return OkOrNotFound(book);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest req, CancellationToken ct)
    {
        var book = await _commands.CreateAsync(req.Title, req.CategoryId, req.AuthorPartyId, req.Copies, ct);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookRequest req, CancellationToken ct)
    {
        var book = await _commands.UpdateAsync(id, req.Title, req.CategoryId, ct);
        return OkOrNotFound(book);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _commands.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
