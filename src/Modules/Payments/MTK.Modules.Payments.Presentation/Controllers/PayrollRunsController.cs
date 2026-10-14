using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Payroll.Commands.CalculatePayrollRun;
using MTK.Modules.Payments.Application.Payroll.Queries.GetPayrollRunById;

namespace MTK.Modules.Payments.Presentation.Controllers;

[RequireAnyRole(Roles.Admin, Roles.Accountant)]
public class PayrollRunsController(ISender sender) : BaseController(sender)
{
    [HttpPost("calculate")]
    public async Task<IActionResult> Calculate(
        [FromBody] CalculatePayrollRunCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Əmək haqqı hesablandı")
            : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPayrollRunByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }
}
