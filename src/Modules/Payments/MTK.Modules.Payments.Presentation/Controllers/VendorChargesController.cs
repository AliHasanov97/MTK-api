using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.VendorCharges.Commands.CancelVendorCharge;
using MTK.Modules.Payments.Application.VendorCharges.Queries.GetChargesByVendor;
using MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class VendorChargesController(ISender sender) : BaseController(sender)
{
    [HttpGet("vendor/{vendorId:guid}")]
    public async Task<IActionResult> GetChargesByVendor(
        Guid vendorId,
        CancellationToken cancellationToken)
    {
        var query = new GetChargesByVendorQuery(vendorId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

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

    [HttpPost("{chargeId:guid}/cancel")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
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
