using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Identity.Application.Users.AssignRolesToUser;
using MTK.Modules.Identity.Application.Users.DeleteUser;
using MTK.Modules.Identity.Application.Users.GetAllUsers;
using MTK.Modules.Identity.Application.Users.GetCurrentUser;
using MTK.Modules.Identity.Application.Users.GetUserById;
using MTK.Modules.Identity.Application.Users.GetUserGroups;
using MTK.Modules.Identity.Application.Users.GetUserRoles;
using MTK.Modules.Identity.Application.Users.GetUserRolesAndGroups;
using MTK.Modules.Identity.Application.Users.RegisterUser;
using MTK.Modules.Identity.Application.Users.RemoveRolesFromUser;
using MTK.Modules.Identity.Application.Users.SearchUsers;
using MTK.Modules.Identity.Application.Users.UpdateUser;

namespace MTK.Modules.Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [RequireAnyRole(Roles.Admin)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPost("search")]
    [RequireAnyRole(Roles.Admin)]
    public async Task<IActionResult> SearchUsers([FromBody] SearchUsersQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPost("register")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        // Admin yaratdığı istifadəçiyə istənilən rolu verə bilər; komandant isə yalnız
        // sakin (owner) hesabı yarada bilər — başqa rol (admin, mühasib, işçi və s.)
        // tələb etsə, bloklanır.
        if (!User.IsInRole(Roles.Admin) && (command.RoleNames is not { Length: 1 } || command.RoleNames[0] != Roles.Owner))
        {
            return BadRequest(new { error = "Komandant yalnız sakin (owner) hesabı yarada bilər." });
        }

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(new { userId = result.Value });
    }

    [HttpPatch("{id:guid}")]
    [RequireAnyRole(Roles.Admin)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { Id = id }, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequireAnyRole(Roles.Admin)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpGet("{userId:guid}/roles")]
    [Authorize]
    public async Task<IActionResult> GetUserRoles(Guid userId, [FromQuery] bool includeInheritedRoles = false, CancellationToken cancellationToken = default)
    {
        var query = new GetUserRolesQuery(userId, includeInheritedRoles);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPost("{userId:guid}/roles")]
    [Authorize]
    public async Task<IActionResult> AssignRolesToUser(Guid userId, [FromBody] AssignRolesToUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { UserId = userId }, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpDelete("{userId:guid}/roles")]
    [RequireAnyRole(Roles.Admin)]
    public async Task<IActionResult> RemoveRolesFromUser(Guid userId, [FromBody] RemoveRolesFromUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { UserId = userId }, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpGet("{userId:guid}/groups")]
    [Authorize]
    public async Task<IActionResult> GetUserGroups(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserGroupsQuery(userId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpGet("{userId:guid}/roles-and-groups")]
    [Authorize]
    public async Task<IActionResult> GetUserRolesAndGroups(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserRolesAndGroupsQuery(userId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var query = new GetCurrentUserQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }
}
