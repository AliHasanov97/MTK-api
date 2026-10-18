using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Common.Domain.Queries;
using SortDirection = MTK.Common.Domain.Queries.SortDirection;
using MTK.Modules.Payments.Application.SalaryPayments.Commands.CreateSalaryPayment;
using MTK.Modules.Payments.Application.SalaryPayments.Commands.DeleteSalaryPayment;
using MTK.Modules.Payments.Application.SalaryPayments.Commands.PaySalaryPayment;
using MTK.Modules.Payments.Application.SalaryPayments.Queries.ExportSalaryPaymentPdf;
using MTK.Modules.Payments.Application.SalaryPayments.Queries.GetSalaryCalculationPreview;
using MTK.Modules.Payments.Application.SalaryPayments.Queries.GetSalaryPaymentById;
using MTK.Modules.Payments.Application.SalaryPayments.Queries.SearchSalaryPayments;

namespace MTK.Modules.Payments.Presentation.Controllers;

[Route("api/salary-payments")]
[RequireAnyRole(Roles.Admin, Roles.Accountant)]
public class SalaryPaymentsController(ISender sender) : BaseController(sender)
{
    /// <summary>
    /// Maaş hesablaması preview et (yaratmadan əvvəl).
    /// </summary>
    [HttpGet("preview")]
    public async Task<IActionResult> Preview(
        [FromQuery] Guid employeeId,
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetSalaryCalculationPreviewQuery(employeeId, month, year),
            cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Yeni maaş ödənişi yarat (Draft statusunda).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSalaryPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Created($"/api/salary-payments/{result.Value.Id}", result.Value, "Maaş ödənişi yaradıldı")
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Maaş ödənişini ID-yə görə gətir.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSalaryPaymentByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Maaş ödənişlərini axtarış et.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] List<QueryFilter>? filters,
        [FromQuery] string? sortField,
        [FromQuery] string? sortDirection,
        [FromQuery] string? searchTerm,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        SortCriteria? sortCriteria = null;
        if (!string.IsNullOrWhiteSpace(sortField))
        {
            sortCriteria = new SortCriteria
            {
                ColumnName = sortField,
                Direction = sortDirection?.ToLower() == "desc"
                    ? SortDirection.Descending
                    : SortDirection.Ascending
            };
        }

        var result = await _sender.Send(
            new SearchSalaryPaymentsQuery(filters, sortCriteria, searchTerm, page, pageSize),
            cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Maaş ödənişini ödə (Draft → Paid).
    /// Transaction-lar yaradılır və şirkət balansı yenilənir.
    /// </summary>
    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> Pay(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new PaySalaryPaymentCommand(id), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Maaş ödənişi ödənildi")
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Maaş ödənişini sil (yalnız Draft statusunda).
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteSalaryPaymentCommand(id), cancellationToken);

        return result.IsSuccess
            ? Success("Maaş ödənişi silindi")
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Maaş ödənişi PDF-i çıxarış.
    /// </summary>
    [HttpGet("{id:guid}/export-pdf")]
    public async Task<IActionResult> ExportPdf(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ExportSalaryPaymentPdfQuery(id), cancellationToken);

        return result.IsSuccess
            ? File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : BadRequest(result.Error);
    }
}
