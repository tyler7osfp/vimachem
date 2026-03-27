using Library.Application.Requests;
using Library.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartiesController(
    IPartyCommandService _commands,
    IPartyRoleCommandService _roleCommands,
    IPartyQueryService _queries) : LibraryControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _queries.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromQuery] bool includeAuthoredBooks = false,
        [FromQuery] bool includeActiveBorrowings = false,
        [FromQuery] bool includePastBorrowings = false,
        CancellationToken ct = default)
    {
        var detail = await _queries.GetByIdAsync(id, includeAuthoredBooks, includeActiveBorrowings, includePastBorrowings, ct);
        return OkOrNotFound(detail);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartyRequest req, CancellationToken ct)
    {
        var party = await _commands.CreateAsync(req.Name, req.Email, req.InitialRole, ct);
        return CreatedAtAction(nameof(GetById), new { id = party.Id }, party);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePartyRequest req, CancellationToken ct)
    {
        var party = await _commands.UpdateAsync(id, req.Name, req.Email, ct);
        return OkOrNotFound(party);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _commands.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/roles")]
    public async Task<IActionResult> AddRole(Guid id, [FromBody] string role, CancellationToken ct)
    {
        await _roleCommands.AddRoleAsync(id, role, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/roles/{role}")]
    public async Task<IActionResult> RemoveRole(Guid id, string role, CancellationToken ct)
    {
        var removed = await _roleCommands.RemoveRoleAsync(id, role, ct);
        return removed ? NoContent() : NotFound();
    }
}
