using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Identity.Application.Users.GetCurrentUser;

namespace MTK.Modules.Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> UserInfo(CancellationToken cancellationToken)
    {
        var query = new GetCurrentUserQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    // NOTE: Login, Logout, and RefreshToken endpoints would typically be handled
    // by Keycloak directly or through a separate authentication service.
    // These can be added later if needed.
}
