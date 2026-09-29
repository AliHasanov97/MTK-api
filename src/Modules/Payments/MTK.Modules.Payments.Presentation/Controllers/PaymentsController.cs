using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Domain.Queries;
using MTK.Modules.Payments.Application.Payments.Commands.CancelPayment;
using MTK.Modules.Payments.Application.Payments.Commands.CreatePayment;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentAllocations;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByProperty;
using MTK.Modules.Payments.Application.Payments.Queries.SearchPayments;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class PaymentsController(ISender sender) : BaseController(sender)
{
    [HttpGet("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetPaymentsByOwner(
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentsByOwnerQuery(ownerId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("property/{propertyId:guid}")]
    public async Task<IActionResult> GetPaymentsByProperty(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentsByPropertyQuery(propertyId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchPayments(
        [FromBody] SearchPaymentsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new SearchPaymentsQuery(
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
    public async Task<IActionResult> CreatePayment(
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand(
            request.OwnerId,
            request.Amount,
            request.PaymentMethod,
            request.PaymentDate,
            request.Reference,
            request.Notes,
            request.PropertyId,
            request.PropertyType);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Ödəniş uğurla qeyd edildi")
            : BadRequest(result.Error);
    }

    [HttpGet("{paymentId:guid}/allocations")]
    public async Task<IActionResult> GetPaymentAllocations(
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentAllocationsQuery(paymentId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("{paymentId:guid}/cancel")]
    public async Task<IActionResult> CancelPayment(
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        var command = new CancelPaymentCommand(paymentId);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success("Ödəniş ləğv edildi")
            : BadRequest(result.Error);
    }
}

public sealed record SearchPaymentsRequest(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize);

public sealed record CreatePaymentRequest(
    Guid OwnerId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    string? Reference,
    string? Notes,
    Guid? PropertyId = null,
    PropertyType? PropertyType = null);
