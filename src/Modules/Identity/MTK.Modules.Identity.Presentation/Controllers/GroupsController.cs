using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Identity.Application.Groups.AddUserToGroup;
using MTK.Modules.Identity.Application.Groups.AssignRolesToGroup;
using MTK.Modules.Identity.Application.Groups.CreateGroup;
using MTK.Modules.Identity.Application.Groups.DeleteGroup;
using MTK.Modules.Identity.Application.Groups.GetGroupById;
using MTK.Modules.Identity.Application.Groups.GetGroupRoleNames;
using MTK.Modules.Identity.Application.Groups.GetGroupRoles;
using MTK.Modules.Identity.Application.Groups.GetGroupUsers;
using MTK.Modules.Identity.Application.Groups.GetGroups;
using MTK.Modules.Identity.Application.Groups.RemoveRolesFromGroup;
using MTK.Modules.Identity.Application.Groups.RemoveUserFromGroup;
using MTK.Modules.Identity.Application.Groups.SearchGroups;
using MTK.Modules.Identity.Application.Groups.UpdateGroup;

namespace MTK.Modules.Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/groups")]
public class GroupsController : ControllerBase
{
    private readonly ISender _sender;

    public GroupsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetGroups(CancellationToken cancellationToken)
    {
        var query = new GetGroupsQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPost("search")]
    [Authorize]
    public async Task<IActionResult> SearchGroups([FromBody] SearchGroupsRequest request, CancellationToken cancellationToken)
    {
        var query = new SearchGroupsQuery(
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
    public async Task<IActionResult> GetGroupById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetGroupByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateGroupCommand(
            request.Name,
            request.Description,
            request.ParentGroupId);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(new { groupId = result.Value });
    }

    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateGroup(Guid id, [FromBody] UpdateGroupRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateGroupCommand(
            id,
            request.Name,
            request.Description,
            request.ParentGroupId);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteGroup(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteGroupCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpGet("rolenames")]
    [Authorize]
    public async Task<IActionResult> GetRealmRoleNames(CancellationToken cancellationToken)
    {
        var query = new GetGroupRoleNamesQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpGet("{groupId:guid}/roles")]
    [Authorize]
    public async Task<IActionResult> GetGroupRoles(Guid groupId, CancellationToken cancellationToken)
    {
        var query = new GetGroupRolesQuery(groupId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPost("roles")]
    [Authorize]
    public async Task<IActionResult> AssignRolesToGroup([FromBody] AssignRolesToGroupRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignRolesToGroupCommand(
            request.GroupId,
            request.RoleNames);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpDelete("{groupId:guid}/roles")]
    [Authorize]
    public async Task<IActionResult> RemoveRolesFromGroup(Guid groupId, [FromBody] RemoveRolesFromGroupRequest request, CancellationToken cancellationToken)
    {
        var command = new RemoveRolesFromGroupCommand(
            groupId,
            request.RoleNames);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpPost("{groupId:guid}/users/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> AddUserToGroup(Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        var command = new AddUserToGroupCommand(groupId, userId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpDelete("{groupId:guid}/users/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> RemoveUserFromGroup(Guid groupId, Guid userId, CancellationToken cancellationToken)
    {
        var command = new RemoveUserFromGroupCommand(groupId, userId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpGet("{groupId:guid}/users")]
    [Authorize]
    public async Task<IActionResult> GetGroupUsers(Guid groupId, CancellationToken cancellationToken)
    {
        var query = new GetGroupUsersQuery(groupId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }
}

// Request DTOs
public sealed record CreateGroupRequest(
    string Name,
    string? Description,
    Guid? ParentGroupId);

public sealed record UpdateGroupRequest(
    string Name,
    string? Description,
    Guid? ParentGroupId);

public sealed record SearchGroupsRequest(
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortDirection = null);

public sealed record AssignRolesToGroupRequest(
    Guid GroupId,
    List<string> RoleNames);

public sealed record RemoveRolesFromGroupRequest(List<string> RoleNames);
