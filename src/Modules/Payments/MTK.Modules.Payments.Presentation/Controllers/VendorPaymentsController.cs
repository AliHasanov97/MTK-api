using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByVendor;
using MTK.Modules.Payments.Application.VendorPayments.Commands.CreateVendorPayment;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class VendorPaymentsController(ISender sender) : BaseController(sender)
{
    [HttpGet("vendor/{vendorId:guid}")]
    public async Task<IActionResult> GetPaymentsByVendor(
        Guid vendorId,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentsByVendorQuery(vendorId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager, Roles.Accountant)]
    public async Task<IActionResult> CreateVendorPayment(
        [FromBody] CreateVendorPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Ödəniş uğurla qeydə alındı")
            : BadRequest(result.Error);
    }
}
