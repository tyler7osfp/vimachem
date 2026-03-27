using Library.Application.Requests;
using Library.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryCommandService _commands, ICategoryQueryService _queries) : LibraryControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _queries.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var cat = await _queries.GetByIdAsync(id, ct);
        return OkOrNotFound(cat);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest req, CancellationToken ct)
    {
        var cat = await _commands.CreateAsync(req.Name, ct);
        return CreatedAtAction(nameof(GetById), new { id = cat.Id }, cat);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest req, CancellationToken ct)
    {
        var cat = await _commands.UpdateAsync(id, req.Name, ct);
        return OkOrNotFound(cat);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _commands.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
