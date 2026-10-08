using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Payments.Application.Nomenclatures.Queries.SearchNomenclatureShadows;

namespace MTK.Modules.Payments.Presentation.Controllers;

/// <summary>
/// Warehouse-dan sync olunan nomenklatura güzgüsü — alış sətirlərində nomenklatura
/// seçmək üçün (paginated search).
/// </summary>
public class NomenclaturesController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchNomenclatures(
        [FromBody] SearchNomenclatureShadowsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }
}
