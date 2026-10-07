using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/warehouse/[controller]")]
[Authorize]
public class BaseController : ControllerBase
{
    protected readonly ISender _sender;

    protected BaseController(ISender sender)
    {
        _sender = sender;
    }

    protected IActionResult Success<T>(T data, string message = "Uğurla icra edildi")
    {
        return Ok(new { data, message });
    }

    protected IActionResult Success(string message = "Uğurla icra edildi")
    {
        return Ok(new { message });
    }

    protected IActionResult BadRequest(Error error)
    {
        return base.BadRequest(new { error = error.Message, code = error.Code });
    }
}
