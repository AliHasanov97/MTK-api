using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Buildings.Application.Apartments.Commands.AssignOwnerToApartment;
using MTK.Modules.Buildings.Application.Apartments.Commands.CreateApartment;
using MTK.Modules.Buildings.Application.Apartments.Commands.TransferApartmentOwnership;
using MTK.Modules.Buildings.Application.Apartments.Commands.UpdateApartment;
using MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentById;
using MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentsByBuilding;

namespace MTK.Modules.Buildings.Presentation.Controllers;

public class ApartmentsController(ISender sender) : BaseController(sender)
{
    [Produces<ApartmentResponse>]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetApartmentById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetApartmentByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [Produces<List<ApartmentResponse>>]
    [HttpGet("building/{buildingId:guid}")]
    public async Task<IActionResult> GetApartmentsByBuilding(
        Guid buildingId,
        CancellationToken cancellationToken)
    {
        var query = new GetApartmentsByBuildingQuery(buildingId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Produces<Guid>]
    [HttpPost]
    public async Task<IActionResult> CreateApartment(
        [FromBody] CreateApartmentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetApartmentById),
                new { id = result.Value },
                result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateApartment(
        Guid id,
        [FromBody] UpdateApartmentCommand command,
        CancellationToken cancellationToken)
    {
        command.ApartmentId = id;

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/assign-owner")]
    public async Task<IActionResult> AssignOwner(
        Guid id,
        [FromBody] AssignOwnerToApartmentCommand command,
        CancellationToken cancellationToken)
    {
        command.ApartmentId = id;

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/transfer-ownership")]
    public async Task<IActionResult> TransferOwnership(
        Guid id,
        [FromBody] TransferApartmentOwnershipCommand command,
        CancellationToken cancellationToken)
    {
        command.ApartmentId = id;

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
