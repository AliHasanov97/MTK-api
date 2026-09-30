using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Modules.Payments.Application.VendorPayments.Commands.CreateVendorPayment;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class VendorPaymentsController(ISender sender) : BaseController(sender)
{
    [HttpPost]
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
