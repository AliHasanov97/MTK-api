using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Payments.Application.OwnerBalances.Commands.RecalculateOwnerBalance;
using MTK.Modules.Payments.Application.OwnerBalances.Queries.GetOwnerBalance;
using MTK.Modules.Payments.Application.OwnerBalances.Queries.GetPropertyBalance;

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

    [HttpGet("property/{propertyId:guid}")]
    public async Task<IActionResult> GetPropertyBalance(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var query = new GetPropertyBalanceQuery(propertyId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : NotFound(result.Error);
    }

    /// <summary>
    /// Balans proyeksiyasını aqreqatdan yenidən hesablayır və düzəlişi qaytarır.
    /// Normal axında balans hər dəyişiklikdən sonra onsuz da mütləq yenidən
    /// hesablandığı üçün bu, keçmiş fərqləri düzəltmək/yoxlamaq üçündür.
    /// </summary>
    [HttpPost("owner/{ownerId:guid}/recalculate")]
    public async Task<IActionResult> RecalculateOwnerBalance(
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var command = new RecalculateOwnerBalanceCommand(ownerId);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : NotFound(result.Error);
    }
}
