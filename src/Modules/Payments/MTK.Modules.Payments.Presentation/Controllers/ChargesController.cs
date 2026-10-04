using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Charges.Commands.CreateCharge;
using MTK.Modules.Payments.Application.Charges.Commands.CreateOneTimeServiceExpense;
using MTK.Modules.Payments.Application.Charges.Queries.ExportAnnualPaymentReport;
using MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;
using MTK.Modules.Payments.Application.Charges.Queries.GetChargeAllocations;
using MTK.Modules.Payments.Application.Charges.Queries.GetChargesByOwner;
using MTK.Modules.Payments.Application.Charges.Queries.SearchCharges;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class ChargesController(ISender sender) : BaseController(sender)
{
    [HttpGet("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetChargesByOwner(
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var query = new GetChargesByOwnerQuery(ownerId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{chargeId:guid}/allocations")]
    public async Task<IActionResult> GetChargeAllocations(
        Guid chargeId,
        CancellationToken cancellationToken)
    {
        var query = new GetChargeAllocationsQuery(chargeId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchCharges(
        [FromBody] SearchChargesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>Bütün mənzil/qarajların bir il üzrə aylıq ödəniş qrafiki — Hesabatlar səhifəsi üçün.</summary>
    [HttpGet("reports/annual")]
    public async Task<IActionResult> GetAnnualPaymentReport(
        [FromQuery] int year,
        [FromQuery] PropertyType? propertyType,
        CancellationToken cancellationToken)
    {
        var query = new GetAnnualPaymentReportQuery(year, propertyType);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>Yuxarıdakı hesabatın Excel (.xlsx) faylı kimi ixracı.</summary>
    [HttpGet("reports/annual/export")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager, Roles.Accountant)]
    public async Task<IActionResult> ExportAnnualPaymentReport(
        [FromQuery] int year,
        [FromQuery] PropertyType? propertyType,
        CancellationToken cancellationToken)
    {
        var query = new ExportAnnualPaymentReportQuery(year, propertyType);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? File(result.Value.FileContent, result.Value.ContentType, result.Value.FileName)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateCharge(
        [FromBody] CreateChargeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Haqq uğurla yaradıldı")
            : BadRequest(result.Error);
    }

    /// <summary>Müqavilənin birdəfəlik xidmətinə qarşı xərc daxil edir — borc yaranır və eyni anda ödənilir.</summary>
    [HttpPost("one-time-service-expense")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateOneTimeServiceExpense(
        [FromBody] CreateOneTimeServiceExpenseCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Xərc uğurla daxil edildi")
            : BadRequest(result.Error);
    }
}
