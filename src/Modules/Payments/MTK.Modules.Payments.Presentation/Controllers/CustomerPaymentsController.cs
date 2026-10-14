using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.CustomerPayments.Commands.CreateCustomerPayment;
using MTK.Modules.Payments.Application.CustomerPayments.Queries.GetPaymentsByCustomer;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class CustomerPaymentsController(ISender sender) : BaseController(sender)
{
    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetPaymentsByCustomer(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPaymentsByCustomerQuery(customerId), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager, Roles.Accountant)]
    public async Task<IActionResult> CreateCustomerPayment(
        [FromBody] CreateCustomerPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Ödəniş uğurla qeydə alındı")
            : BadRequest(result.Error);
    }
}
