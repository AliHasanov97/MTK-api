using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Identity.Application.Profile.ChangeUserInfo;

namespace MTK.Modules.Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/profile")]
public class ProfileController : ControllerBase
{
    private readonly ISender _sender;

    public ProfileController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPatch("change-info")]
    [Authorize]
    public async Task<IActionResult> ChangeUserInfo([FromBody] ChangeUserInfoCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }
}
