using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Buildings.Application.Owners.Commands.CreatePassiveOwner;
using MTK.Modules.Buildings.Application.Owners.Queries.GetOwnerById;

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

    /// <summary>
    /// User account olmadan passive owner yarat
    /// Bu owner-lər yalnız məlumat saxlamaq üçündür (invoice, debt tracking)
    /// </summary>
    [HttpPost("passive")]
    public async Task<IActionResult> CreatePassiveOwner(
        [FromBody] CreatePassiveOwnerRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePassiveOwnerCommand(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Email,
            request.Notes);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetOwnerById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }
}

public sealed record CreatePassiveOwnerRequest(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string? Notes = null);
