using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Buildings.Application.ApartmentTypes;
using MTK.Modules.Buildings.Application.ApartmentTypes.Commands.CreateApartmentType;
using MTK.Modules.Buildings.Application.ApartmentTypes.Queries.GetApartmentTypesByBuilding;
using MTK.Modules.Buildings.Application.Buildings.Commands.CreateBuilding;
using MTK.Modules.Buildings.Application.Buildings.Queries.GetAllBuildings;
using MTK.Modules.Buildings.Application.Buildings.Queries.GetBuildingById;

namespace MTK.Modules.Buildings.Presentation.Controllers;

public class BuildingsController(ISender sender) : BaseController(sender)
{
    [Produces<List<BuildingResponse>>]
    [HttpGet]
    public async Task<IActionResult> GetAllBuildings(CancellationToken cancellationToken)
    {
        var query = new GetAllBuildingsQuery();
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Produces<BuildingResponse>]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBuildingById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBuildingByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [Produces<List<ApartmentTypeResponse>>]
    [HttpGet("{id:guid}/apartment-types")]
    public async Task<IActionResult> GetApartmentTypes(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetApartmentTypesByBuildingQuery(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Produces<Guid>]
    [HttpPost("{id:guid}/apartment-types")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateApartmentType(
        Guid id,
        [FromBody] CreateApartmentTypeCommand command,
        CancellationToken cancellationToken)
    {
        command.BuildingId = id;

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Produces<Guid>]
    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateBuilding(
        [FromBody] CreateBuildingCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetBuildingById),
                new { id = result.Value },
                result.Value);
        }

        return BadRequest(result.Error);
    }
}
