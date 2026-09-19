using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Identity.Application.Roles.CreateRealmRole;
using MTK.Modules.Identity.Application.Roles.DeleteRole;
using MTK.Modules.Identity.Application.Roles.GetAllRoles;
using MTK.Modules.Identity.Application.Roles.GetRoleAssignments;
using MTK.Modules.Identity.Application.Roles.GetRoleById;
using MTK.Modules.Identity.Application.Roles.GetRoleByName;
using MTK.Modules.Identity.Application.Roles.SearchRoles;
using MTK.Modules.Identity.Application.Roles.UpdateRole;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Presentation.Controllers;

[ApiController]
[Route("api/identity/roles")]
public class RolesController : ControllerBase
{
    private readonly ISender _sender;

    public RolesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateRealmRole([FromBody] CreateRealmRoleRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateRealmRoleCommand(
            request.Name,
            request.Description,
            request.RoleType);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(new { roleId = result.Value });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
    {
        var query = new GetAllRolesQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetRoleByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpGet("by-name/{name}")]
    [Authorize]
    public async Task<IActionResult> GetRoleByName(string name, CancellationToken cancellationToken)
    {
        var query = new GetRoleByNameQuery(name);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPost("search")]
    [Authorize]
    public async Task<IActionResult> SearchRoles([FromBody] SearchRolesRequest request, CancellationToken cancellationToken)
    {
        var query = new SearchRolesQuery(
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

    [HttpGet("{id:guid}/assignments")]
    [Authorize]
    public async Task<IActionResult> GetRoleAssignments(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetRoleAssignmentsQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateRoleCommand(
            id,
            request.Name,
            request.Description,
            request.IsActive);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteRoleCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        return NoContent();
    }
}

// Request DTOs
public sealed record CreateRealmRoleRequest(
    string Name,
    string? Description,
    RoleType RoleType);

public sealed record UpdateRoleRequest(
    string Name,
    string? Description,
    bool IsActive);

public sealed record SearchRolesRequest(
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortDirection = null);
