using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Common.Domain.Queries;
using MTK.Modules.Buildings.Application.Apartments.Commands.AssignOwnerToApartment;
using MTK.Modules.Buildings.Application.Apartments.Commands.CreateApartment;
using MTK.Modules.Buildings.Application.Apartments.Commands.TransferApartmentOwnership;
using MTK.Modules.Buildings.Application.Apartments.Commands.UpdateApartment;
using MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentById;
using MTK.Modules.Buildings.Application.Apartments.Queries.GetApartmentsByBuilding;
using MTK.Modules.Buildings.Application.Apartments.Queries.SearchApartments;

namespace MTK.Modules.Buildings.Presentation.Controllers;

public class ApartmentsController(ISender sender) : BaseController(sender)
{
    [Produces<SearchApartmentsResponse>]
    [HttpPost("search")]
    public async Task<IActionResult> SearchApartments(
        [FromBody] SearchApartmentsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new SearchApartmentsQuery(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize);

        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

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
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
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
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
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
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
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
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
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
