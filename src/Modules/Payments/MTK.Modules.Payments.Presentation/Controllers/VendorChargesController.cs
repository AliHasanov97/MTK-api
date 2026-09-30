using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Payments.Application.VendorCharges.Commands.CancelVendorCharge;
using MTK.Modules.Payments.Application.VendorCharges.Commands.RecordGoodsDelivery;
using MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class VendorChargesController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchVendorCharges(
        [FromBody] SearchVendorChargesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>Mal tədarükünü (qaimə) qeydə alır — borc yaradır.</summary>
    [HttpPost("goods-delivery")]
    public async Task<IActionResult> RecordGoodsDelivery(
        [FromBody] RecordGoodsDeliveryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Tədarük qeydə alındı, borc yaradıldı")
            : BadRequest(result.Error);
    }

    [HttpPost("{chargeId:guid}/cancel")]
    public async Task<IActionResult> CancelVendorCharge(
        Guid chargeId,
        [FromBody] CancelVendorChargeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { ChargeId = chargeId }, cancellationToken);

        return result.IsSuccess
            ? Success("Borc ləğv edildi")
            : BadRequest(result.Error);
    }
}
