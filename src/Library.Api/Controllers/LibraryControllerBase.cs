using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

public abstract class LibraryControllerBase : ControllerBase
{
    protected IActionResult OkOrNotFound<T>(T? result) where T : class
        => result is null ? NotFound() : Ok(result);
}
