using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Payments.Application.OwnerBalances.Queries.GetOwnerBalance;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class BalancesController(ISender sender) : BaseController(sender)
{
    [HttpGet("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetOwnerBalance(
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var query = new GetOwnerBalanceQuery(ownerId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : NotFound(result.Error);
    }
}
