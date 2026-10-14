using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.CustomerCharges.Commands.CancelCustomerCharge;
using MTK.Modules.Payments.Application.CustomerCharges.Commands.CreateCustomerCharge;
using MTK.Modules.Payments.Application.CustomerCharges.Queries.GetChargesByCustomer;
using MTK.Modules.Payments.Application.CustomerCharges.Queries.SearchCustomerCharges;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class CustomerChargesController(ISender sender) : BaseController(sender)
{
    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetChargesByCustomer(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetChargesByCustomerQuery(customerId), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchCustomerCharges(
        [FromBody] SearchCustomerChargesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager, Roles.Accountant)]
    public async Task<IActionResult> CreateCustomerCharge(
        [FromBody] CreateCustomerChargeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Borc uğurla yazıldı")
            : BadRequest(result.Error);
    }

    [HttpPost("{chargeId:guid}/cancel")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CancelCustomerCharge(
        Guid chargeId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CancelCustomerChargeCommand(chargeId), cancellationToken);

        return result.IsSuccess
            ? Success("Borc ləğv edildi")
            : BadRequest(result.Error);
    }
}
