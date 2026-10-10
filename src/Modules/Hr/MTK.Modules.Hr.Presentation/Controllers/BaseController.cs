using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Presentation.Filters;

namespace MTK.Modules.Hr.Presentation.Controllers;

[ApiController]
[Route("api/hr/[controller]")]
[Authorize]
[TypeFilter(typeof(HrDbExceptionFilter))]
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

    protected IActionResult NotFound(Error error)
    {
        return base.NotFound(new { error = error.Message, code = error.Code });
    }
}
