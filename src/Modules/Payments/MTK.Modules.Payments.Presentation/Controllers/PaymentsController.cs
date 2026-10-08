using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Payments.Commands.CreatePayment;
using MTK.Modules.Payments.Application.Payments.Queries.ExportPaymentReceipt;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentAllocations;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByOwner;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentById;
using MTK.Modules.Payments.Application.Payments.Queries.GetPaymentsByProperty;
using MTK.Modules.Payments.Application.Payments.Queries.SearchPayments;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class PaymentsController(ISender sender) : BaseController(sender)
{
    [HttpGet("{paymentId:guid}")]
    public async Task<IActionResult> GetPaymentById(Guid paymentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPaymentByIdQuery(paymentId), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

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
        [FromBody] SearchPaymentsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager, Roles.Accountant)]
    public async Task<IActionResult> CreatePayment(
        [FromBody] CreatePaymentCommand command,
        CancellationToken cancellationToken)
    {
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

    /// <summary>
    /// Frontend yalnız paymentId ötürür — qəbzdəki qalan hər şey (sahib/tədarükçü adı,
    /// mənzil/qaraj nömrəsi, dövr, bina ünvanı, əməliyyatı icra edən şəxs/vəzifə) server
    /// tərəfdə, Payments-in öz məlumatından həll olunur.
    /// </summary>
    [HttpGet("{paymentId:guid}/receipt")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager, Roles.Accountant)]
    public async Task<IActionResult> ExportReceipt(Guid paymentId, CancellationToken cancellationToken)
    {
        var query = new ExportPaymentReceiptQuery(paymentId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? File(result.Value.FileContent, result.Value.ContentType, result.Value.FileName)
            : BadRequest(result.Error);
    }
}
