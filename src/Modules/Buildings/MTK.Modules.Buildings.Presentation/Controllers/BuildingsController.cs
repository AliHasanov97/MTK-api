using MediatR;
using Microsoft.AspNetCore.Mvc;
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

    [Produces<Guid>]
    [HttpPost]
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
