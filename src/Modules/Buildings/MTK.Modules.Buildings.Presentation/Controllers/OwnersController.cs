using MediatR;
using Microsoft.AspNetCore.Mvc;
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
}
