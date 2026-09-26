using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Presentation.Responses;

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

    protected IActionResult Success<T>(T data, string message = "Əməliyyat uğurla tamamlandı")
    {
        return Ok(new ResponseObjectWith<T>(data, message));
    }

    protected IActionResult Success(string message = "Əməliyyat uğurla tamamlandı")
    {
        return Ok(new ResponseObject(message));
    }

    protected IActionResult Created<T>(T data, string message = "Uğurla yaradıldı")
    {
        return StatusCode(201, new ResponseObjectWith<T>(data, message, 201));
    }

    protected new IActionResult BadRequest(object? error)
    {
        if (error is Error err)
        {
            return base.BadRequest(new ErrorResponse(err.Message, err.Code));
        }
        return base.BadRequest(error);
    }

    protected new IActionResult NotFound(object? error)
    {
        if (error is Error err)
        {
            return base.NotFound(new ErrorResponse(err.Message, err.Code, 404));
        }
        return base.NotFound(error);
    }
}
