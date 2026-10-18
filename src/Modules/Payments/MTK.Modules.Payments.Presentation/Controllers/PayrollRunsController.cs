using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Payroll.Commands.ApprovePayrollRun;
using MTK.Modules.Payments.Application.Payroll.Commands.CalculatePayrollRun;
using MTK.Modules.Payments.Application.Payroll.Commands.PayPayrollRun;
using MTK.Modules.Payments.Application.Payroll.Commands.PayPayslip;
using MTK.Modules.Payments.Application.Payroll.Commands.RecalculatePayrollRun;
using MTK.Modules.Payments.Application.Payroll.Queries.ExportPayrollRun;
using MTK.Modules.Payments.Application.Payroll.Queries.ExportPayslip;
using MTK.Modules.Payments.Application.Payroll.Queries.GetAllPayrollRuns;
using MTK.Modules.Payments.Application.Payroll.Queries.GetPayrollRunById;
using MTK.Modules.Payments.Application.Payroll.Queries.GetPayrollRunByPeriod;
using MTK.Modules.Payments.Application.Payroll.Queries.GetPayslipsByEmployee;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Presentation.Controllers;

[Route("api/payroll-runs")]
[RequireAnyRole(Roles.Admin, Roles.Accountant)]
public class PayrollRunsController(ISender sender) : BaseController(sender)
{
    /// <summary>Bütün PayrollRun-ları gətir (qəbzlər olmadan - tarixçə üçün).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllPayrollRunsQuery(), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Verilmiş dövr üçün hesablama yaradır. Ay bitməmiş hesablanmır: yalnız keçmiş təqvim ayı
    /// üçün mümkündür (məs. noyabrda yalnız oktyabr) — əks halda PayrollRun.PeriodNotClosed.
    /// </summary>
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

    /// <summary>
    /// Mövcud hesablamanı yenidən qurur (sonradan gələn bonus/düzəlişləri də daxil etmək üçün).
    /// Yalnız heç bir qəbz ödənilməmişsə mümkündür — təsdiqlənmişsə təkrar təsdiq tələb olunur.
    /// </summary>
    [HttpPost("{id:guid}/recalculate")]
    public async Task<IActionResult> Recalculate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RecalculatePayrollRunCommand(id), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Əmək haqqı yenidən hesablandı")
            : BadRequest(result.Error);
    }

    /// <summary>Seçilmiş ay üçün artıq hesablanmış PayrollRun (UI-da ay seçəndə baxış üçün; hesablanmayıbsa 400 + "NotFound" kodu).</summary>
    [HttpGet("by-period")]
    public async Task<IActionResult> GetByPeriod([FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPayrollRunByPeriodQuery(year, month), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
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

    /// <summary>Hesablamanın rəqəmlərini təsdiqləyir — ödəniş yalnız bundan sonra mümkündür.</summary>
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ApprovePayrollRunCommand(id), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Əmək haqqı hesablaması təsdiqləndi")
            : BadRequest(result.Error);
    }

    /// <summary>Bir işçinin qəbzini ödənilmiş qeyd edir və uyğun Transaction (xərc) yaradır.</summary>
    [HttpPost("{id:guid}/payslips/{employeeId:guid}/pay")]
    public async Task<IActionResult> PayPayslip(
        Guid id,
        Guid employeeId,
        [FromBody] PayPayslipRequest? request,
        CancellationToken cancellationToken)
    {
        var command = new PayPayslipCommand(id, employeeId, request?.PaymentMethod ?? PaymentMethod.BankTransfer);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Əmək haqqı ödənildi")
            : BadRequest(result.Error);
    }

    /// <summary>Hələ ödənilməmiş bütün qəbzləri ödənilmiş qeyd edir — hər biri üçün ayrıca Transaction yaradır.</summary>
    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> PayAll(
        Guid id,
        [FromBody] PayPayslipRequest? request,
        CancellationToken cancellationToken)
    {
        var command = new PayPayrollRunCommand(id, request?.PaymentMethod ?? PaymentMethod.BankTransfer);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Bütün əmək haqları ödənildi")
            : BadRequest(result.Error);
    }

    /// <summary>İşçinin bütün payslip-lərini gətir (bütün PayrollRun-lardan).</summary>
    [HttpGet("payslips/employee/{employeeId:guid}")]
    public async Task<IActionResult> GetPayslipsByEmployee(Guid employeeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPayslipsByEmployeeQuery(employeeId), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }
}

public sealed record PayPayslipRequest(PaymentMethod? PaymentMethod);
