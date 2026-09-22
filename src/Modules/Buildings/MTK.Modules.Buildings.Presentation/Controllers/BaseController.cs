using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Presentation.Controllers;

[ApiController]
[Route("api/buildings/[controller]")]
[Authorize]
public class BaseController : ControllerBase
{
    protected readonly ISender _sender;

    public BaseController(ISender sender)
    {
        _sender = sender;
    }

    protected new IActionResult BadRequest(object? error)
    {
        if (error is Error err)
        {
            return base.BadRequest(new { error = err.Message, code = err.Code });
        }
        return base.BadRequest(error);
    }

    protected new IActionResult NotFound(object? error)
    {
        if (error is Error err)
        {
            return base.NotFound(new { error = err.Message, code = err.Code });
        }
        return base.NotFound(error);
    }
}
