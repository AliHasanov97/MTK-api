using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Customers.Commands.CreateCustomer;
using MTK.Modules.Payments.Application.Customers.Commands.DeleteCustomer;
using MTK.Modules.Payments.Application.Customers.Commands.SetCustomerStatus;
using MTK.Modules.Payments.Application.Customers.Commands.UpdateCustomer;
using MTK.Modules.Payments.Application.Customers.Queries.GetCustomerBalance;
using MTK.Modules.Payments.Application.Customers.Queries.GetCustomerById;
using MTK.Modules.Payments.Application.Customers.Queries.SearchCustomers;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class CustomersController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchCustomers(
        [FromBody] SearchCustomersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{customerId:guid}")]
    public async Task<IActionResult> GetCustomerById(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCustomerByIdQuery(customerId), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{customerId:guid}/balance")]
    public async Task<IActionResult> GetCustomerBalance(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCustomerBalanceQuery(customerId), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateCustomer(
        [FromBody] CreateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Müştəri uğurla yaradıldı")
            : BadRequest(result.Error);
    }

    [HttpPut("{customerId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> UpdateCustomer(
        Guid customerId,
        [FromBody] UpdateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        // Route identifikatoru gövdədəkindən üstündür.
        var result = await _sender.Send(command with { CustomerId = customerId }, cancellationToken);

        return result.IsSuccess
            ? Success("Müştəri uğurla yeniləndi")
            : BadRequest(result.Error);
    }

    [HttpPut("{customerId:guid}/status")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> SetCustomerStatus(
        Guid customerId,
        [FromBody] SetCustomerStatusCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { CustomerId = customerId }, cancellationToken);

        return result.IsSuccess
            ? Success(command.IsActive ? "Müştəri aktivləşdirildi" : "Müştəri dayandırıldı")
            : BadRequest(result.Error);
    }

    [HttpDelete("{customerId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> DeleteCustomer(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteCustomerCommand(customerId), cancellationToken);

        return result.IsSuccess
            ? Success("Müştəri silindi")
            : BadRequest(result.Error);
    }
}
