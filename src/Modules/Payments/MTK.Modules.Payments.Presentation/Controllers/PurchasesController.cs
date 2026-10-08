using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Purchases.Commands.CancelPurchase;
using MTK.Modules.Payments.Application.Purchases.Commands.CreatePurchase;
using MTK.Modules.Payments.Application.Purchases.Commands.DeletePurchase;
using MTK.Modules.Payments.Application.Purchases.Commands.ReceivePurchase;
using MTK.Modules.Payments.Application.Purchases.Commands.UpdatePurchase;
using MTK.Modules.Payments.Application.Purchases.Queries.GetPurchaseById;
using MTK.Modules.Payments.Application.Purchases.Queries.GetNomenclaturePurchaseHistory;
using MTK.Modules.Payments.Application.Purchases.Queries.SearchPurchases;

namespace MTK.Modules.Payments.Presentation.Controllers;

/// <summary>
/// Alış (satınalma) sənədləri. Məhsullar bu modulda alınır; alış qəbul edildikdə
/// Warehouse moduluna stok artımı üçün integration event göndərilir.
/// </summary>
public class PurchasesController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchPurchases(
        [FromBody] SearchPurchasesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{purchaseId:guid}")]
    public async Task<IActionResult> GetPurchaseById(
        Guid purchaseId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPurchaseByIdQuery(purchaseId), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("nomenclature/{nomenclatureId:guid}/history")]
    public async Task<IActionResult> GetNomenclaturePurchaseHistory(
        Guid nomenclatureId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetNomenclaturePurchaseHistoryQuery(nomenclatureId),
            cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreatePurchase(
        [FromBody] CreatePurchaseCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Alış uğurla yaradıldı")
            : BadRequest(result.Error);
    }

    [HttpPut("{purchaseId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> UpdatePurchase(
        Guid purchaseId,
        [FromBody] UpdatePurchaseCommand command,
        CancellationToken cancellationToken)
    {
        // Route identifikatoru gövdədəkindən üstündür.
        var result = await _sender.Send(command with { PurchaseId = purchaseId }, cancellationToken);

        return result.IsSuccess
            ? Success("Alış uğurla yeniləndi")
            : BadRequest(result.Error);
    }

    [HttpPost("{purchaseId:guid}/receive")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> ReceivePurchase(
        Guid purchaseId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ReceivePurchaseCommand(purchaseId), cancellationToken);

        return result.IsSuccess
            ? Success("Alış qəbul edildi və anbara göndərildi")
            : BadRequest(result.Error);
    }

    [HttpPost("{purchaseId:guid}/cancel")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CancelPurchase(
        Guid purchaseId,
        [FromBody] CancelPurchaseCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { PurchaseId = purchaseId }, cancellationToken);

        return result.IsSuccess
            ? Success("Alış ləğv edildi")
            : BadRequest(result.Error);
    }

    [HttpDelete("{purchaseId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> DeletePurchase(
        Guid purchaseId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeletePurchaseCommand(purchaseId), cancellationToken);

        return result.IsSuccess
            ? Success("Alış silindi")
            : BadRequest(result.Error);
    }
}
