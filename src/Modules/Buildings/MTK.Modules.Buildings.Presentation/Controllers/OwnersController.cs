using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Buildings.Application.Owners.Commands.CreatePassiveOwner;
using MTK.Modules.Buildings.Application.Owners.Commands.LinkOwnerToUser;
using MTK.Modules.Buildings.Application.Owners.Queries.GetOwnerById;
using MTK.Modules.Buildings.Application.Owners.Queries.GetOwnerByUserId;
using MTK.Modules.Buildings.Application.Owners.Queries.SearchOwners;

namespace MTK.Modules.Buildings.Presentation.Controllers;

public class OwnersController(ISender sender) : BaseController(sender)
{
    [Produces<OwnerResponse>]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOwnerById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOwnerByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>"Mənim profilim" — sakinin öz Owner qeydini öz UserId-si ilə tapır.</summary>
    [Produces<OwnerResponse>]
    [HttpGet("by-user/{userId:guid}")]
    public async Task<IActionResult> GetOwnerByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetOwnerByUserIdQuery(userId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [Produces<SearchOwnersResponse>]
    [HttpPost("search")]
    public async Task<IActionResult> SearchOwners(
        [FromBody] SearchOwnersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// User account olmadan passive owner yarat
    /// Bu owner-lər yalnız məlumat saxlamaq üçündür (invoice, debt tracking)
    /// </summary>
    [HttpPost("passive")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreatePassiveOwner(
        [FromBody] CreatePassiveOwnerCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetOwnerById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Passiv sahibi mövcud bir istifadəçi hesabına bağlayır
    /// </summary>
    [HttpPost("{id:guid}/link-user")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> LinkOwnerToUser(
        Guid id,
        [FromBody] LinkOwnerToUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { OwnerId = id }, cancellationToken);

        return result.IsSuccess
            ? Success("Sahib hesaba uğurla bağlandı")
            : BadRequest(result.Error);
    }
}
