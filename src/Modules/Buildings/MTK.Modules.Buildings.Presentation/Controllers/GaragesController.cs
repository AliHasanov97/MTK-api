using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Buildings.Application.Garages.Commands.CreateGarage;

namespace MTK.Modules.Buildings.Presentation.Controllers;

public class GaragesController(ISender sender) : BaseController(sender)
{
    [Produces<Guid>]
    [HttpPost]
    public async Task<IActionResult> CreateGarage(
        [FromBody] CreateGarageCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Created(
                $"/api/buildings/garages/{result.Value}",
                result.Value);
        }

        return BadRequest(result.Error);
    }
}
