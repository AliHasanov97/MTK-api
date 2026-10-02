using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Common.Domain.Queries;
using MTK.Modules.Payments.Application.Charges.Commands.CreateCharge;
using MTK.Modules.Payments.Application.Charges.Commands.CreateOneTimeServiceExpense;
using MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;
using MTK.Modules.Payments.Application.Charges.Queries.GetChargeAllocations;
using MTK.Modules.Payments.Application.Charges.Queries.GetChargesByOwner;
using MTK.Modules.Payments.Application.Charges.Queries.SearchCharges;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Payments;

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
        [FromBody] SearchChargesRequest request,
        CancellationToken cancellationToken)
    {
        var query = new SearchChargesQuery(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize);

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

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateCharge(
        [FromBody] CreateChargeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateChargeCommand(
            request.OwnerId,
            request.PropertyType,
            request.PropertyId,
            request.Amount,
            request.Description,
            request.Period);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Haqq uğurla yaradıldı")
            : BadRequest(result.Error);
    }

    /// <summary>Müqavilənin birdəfəlik xidmətinə qarşı xərc daxil edir — borc yaranır və eyni anda ödənilir.</summary>
    [HttpPost("one-time-service-expense")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateOneTimeServiceExpense(
        [FromBody] CreateOneTimeServiceExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOneTimeServiceExpenseCommand(
            request.ContractId,
            request.ContractServiceId,
            request.Amount,
            request.PaymentMethod,
            request.Notes);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Xərc uğurla daxil edildi")
            : BadRequest(result.Error);
    }
}

public sealed record SearchChargesRequest(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize);

public sealed record CreateChargeRequest(
    Guid OwnerId,
    PropertyType PropertyType,
    Guid PropertyId,
    decimal Amount,
    string Description,
    string? Period = null);

public sealed record CreateOneTimeServiceExpenseRequest(
    Guid ContractId,
    Guid ContractServiceId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    string? Notes = null);
