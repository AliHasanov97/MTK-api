using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Domain.Queries;
using MTK.Modules.Payments.Application.Charges.Commands.CreateCharge;
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

    [HttpPost]
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
