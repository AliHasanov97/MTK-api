using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Buildings.Application.Garages.Commands.AssignOwnerToGarage;
using MTK.Modules.Buildings.Application.Garages.Commands.CreateGarage;
using MTK.Modules.Buildings.Application.Garages.Commands.DeleteGarage;
using MTK.Modules.Buildings.Application.Garages.Commands.RemoveOwnerFromGarage;
using MTK.Modules.Buildings.Application.Garages.Commands.UpdateGarage;
using MTK.Modules.Buildings.Application.Garages.Queries.GetAllGarages;
using MTK.Modules.Buildings.Application.Garages.Queries.GetGarageById;
using MTK.Modules.Buildings.Application.Garages.Queries.GetGaragesByOwnerId;
using MTK.Modules.Buildings.Application.Garages.Queries.SearchGarages;

namespace MTK.Modules.Buildings.Presentation.Controllers;

public class GaragesController(ISender sender) : BaseController(sender)
{
    [HttpGet]
    public async Task<IActionResult> GetAllGarages(CancellationToken cancellationToken)
    {
        var query = new GetAllGaragesQuery();
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Bütün qarajlar uğurla əldə edildi")
            : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGarageById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetGarageByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Qaraj uğurla tapıldı")
            : NotFound(result.Error);
    }

    [HttpGet("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetGaragesByOwnerId(
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var query = new GetGaragesByOwnerIdQuery(ownerId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Sahibə aid qarajlar uğurla əldə edildi")
            : BadRequest(result.Error);
    }

    [Produces<SearchGaragesResponse>]
    [HttpPost("search")]
    public async Task<IActionResult> SearchGarages(
        [FromBody] SearchGaragesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateGarage(
        [FromBody] CreateGarageCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Created(result.Value, "Qaraj uğurla yaradıldı")
            : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> UpdateGarage(
        Guid id,
        [FromBody] UpdateGarageCommand command,
        CancellationToken cancellationToken)
    {
        command.GarageId = id;

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success("Qaraj məlumatları uğurla yeniləndi")
            : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/assign-owner")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> AssignOwner(
        Guid id,
        [FromBody] AssignOwnerToGarageCommand command,
        CancellationToken cancellationToken)
    {
        command.GarageId = id;

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success("Sahib uğurla təyin edildi")
            : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/remove-owner")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> RemoveOwner(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new RemoveOwnerFromGarageCommand { GarageId = id };

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success("Sahib uğurla çıxarıldı")
            : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> DeleteGarage(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteGarageCommand { GarageId = id };

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success("Qaraj uğurla silindi")
            : BadRequest(result.Error);
    }
}
