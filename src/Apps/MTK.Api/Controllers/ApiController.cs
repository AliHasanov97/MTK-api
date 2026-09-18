using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Domain.Abstractions;

namespace MTK.Api.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender Sender;

    protected ApiController(ISender sender)
    {
        Sender = sender;
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        return HandleFailure(result);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return HandleFailure(result);
    }

    private IActionResult HandleFailure(Result result)
    {
        return result.Error.Code switch
        {
            "NotFound" => NotFound(new { error = result.Error.Message }),
            _ => BadRequest(new { error = result.Error.Message })
        };
    }
}
