using Contracts;
using EventService.Models;
using EventService.Repos;
using Microsoft.AspNetCore.Mvc;

namespace EventService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController(IEventReadRepository repo) : ControllerBase
    {
        [HttpGet("{entityId:guid}")]
        public async Task<IActionResult> GetByEntityId(
            Guid entityId,
            [FromQuery] PagedListQuery paging,
            CancellationToken ct = default)
        {
            var (items, total) = await repo.GetByEntityIdAsync(entityId, paging.Page, paging.PageSize, ct);
            var itemList = items as IReadOnlyCollection<EventDocument> ?? items.ToArray();
            return Ok(new PagedResponse<EventDocument>(total, paging.Page, paging.PageSize, itemList));
        }

        [HttpGet("type/{entityType}")]
        public async Task<IActionResult> GetByType(
            EntityType entityType,
            [FromQuery] PagedListQuery paging,
            CancellationToken ct = default)
        {
            var (items, total) = await repo.GetByEntityTypeAsync(entityType, paging.Page, paging.PageSize, ct);
            var itemList = items as IReadOnlyCollection<EventDocument> ?? items.ToArray();
            return Ok(new PagedResponse<EventDocument>(total, paging.Page, paging.PageSize, itemList));
        }
    }
}
