using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Payroll.Commands.CalculatePayrollRun;
using MTK.Modules.Payments.Application.Payroll.Queries.ExportPayrollRun;
using MTK.Modules.Payments.Application.Payroll.Queries.ExportPayslip;
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

    [HttpGet("{id:guid}/export")]
    public async Task<IActionResult> ExportRun(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportPayrollRunQuery(id), cancellationToken);

        return result.IsSuccess
            ? File(result.Value.FileContent, result.Value.ContentType, result.Value.FileName)
            : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}/payslips/{employeeId:guid}/export")]
    public async Task<IActionResult> ExportPayslip(Guid id, Guid employeeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportPayslipQuery(id, employeeId), cancellationToken);

        return result.IsSuccess
            ? File(result.Value.FileContent, result.Value.ContentType, result.Value.FileName)
            : BadRequest(result.Error);
    }
}
