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
    [RequireAnyRole(Permissions.UsersView, Permissions.UsersManage)]
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
    [RequireAnyRole(Permissions.UsersView, Permissions.UsersManage,Permissions.UsersCreate)]
    public async Task<IActionResult> SearchUsers([FromBody] SearchUsersRequest request, CancellationToken cancellationToken)
    {
        var query = new SearchUsersQuery(
            request.SearchTerm,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDirection);

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
    [RequireAnyRole(Permissions.UsersCreate)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password,
            request.PhoneNumber,
            request.RoleNames);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(new { userId = result.Value });
    }

    [HttpPatch("{id:guid}")]
    [RequireAnyRole(Permissions.UsersUpdate, Permissions.UsersManage)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(
            id,
            request.FirstName,
            request.LastName,
            request.PhoneNumber);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequireAnyRole(Permissions.UsersDelete, Permissions.UsersManage)]
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
    [RequireAnyRole(Permissions.RolesManage)]
    public async Task<IActionResult> AssignRolesToUser(Guid userId, [FromBody] AssignRolesRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignRolesToUserCommand(userId, request.RoleNames);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpDelete("{userId:guid}/roles")]
    [RequireAnyRole(Permissions.RolesManage)]
    public async Task<IActionResult> RemoveRolesFromUser(Guid userId, [FromBody] RemoveRolesRequest request, CancellationToken cancellationToken)
    {
        var command = new RemoveRolesFromUserCommand(userId, request.RoleNames);
        var result = await _sender.Send(command, cancellationToken);

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

// Request DTOs
public sealed record RegisterUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string? PhoneNumber,
    string[]? RoleNames = null);

public sealed record UpdateUserRequest(
    string FirstName,
    string LastName,
    string? PhoneNumber);

public sealed record SearchUsersRequest(
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortDirection = null);

public sealed record AssignRolesRequest(List<string> RoleNames);

public sealed record RemoveRolesRequest(List<string> RoleNames);
