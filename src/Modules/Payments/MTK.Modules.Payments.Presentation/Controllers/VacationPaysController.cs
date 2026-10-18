using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.VacationPays.Commands.PayVacationPay;
using MTK.Modules.Payments.Application.VacationPays.Queries.GetUpcomingVacationPays;
using MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPayById;
using MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPaysByEmployee;
using MTK.Modules.Payments.Application.VacationPays.Queries.SearchVacationPays;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Presentation.Controllers;

[Route("api/vacation-pays")]
[RequireAnyRole(Roles.Admin, Roles.Accountant)]
public class VacationPaysController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchVacationPays(
        [FromBody] SearchVacationPaysQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("employee/{employeeId:guid}")]
    public async Task<IActionResult> GetVacationPaysByEmployee(Guid employeeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVacationPaysByEmployeeQuery(employeeId), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingVacationPays(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUpcomingVacationPaysQuery(), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetVacationPayById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVacationPayByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> PayVacationPay(Guid id, [FromBody] PayVacationPayRequest request, CancellationToken cancellationToken)
    {
        var command = new PayVacationPayCommand(id, request.PaymentMethod);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success("Məzuniyyət haqqı ödənildi")
            : BadRequest(result.Error);
    }
}

public sealed record PayVacationPayRequest(PaymentMethod PaymentMethod);
