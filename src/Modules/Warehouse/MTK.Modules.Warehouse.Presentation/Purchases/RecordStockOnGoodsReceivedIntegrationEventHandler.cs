using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.IntegrationEvents.Warehouse;
using MTK.Modules.Warehouse.Application.Transactions.Commands.RecordReceiptFromPurchase;

namespace MTK.Modules.Warehouse.Presentation.Purchases;

/// <summary>
/// Payments modulunda alış qəbul edildikdə anbar stokunu artırır. Məhsullar Payments-də
/// alınır — anbara girişin yeganə yoludur bu (manual mal qəbulu yoxdur).
/// </summary>
internal sealed class RecordStockOnGoodsReceivedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<GoodsReceivedIntegrationEvent>
{
    public override async Task Handle(
        GoodsReceivedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var lines = integrationEvent.Lines
            .Select(line => new ReceivedLine(line.NomenclatureId, line.Quantity, line.UnitPrice))
            .ToList();

        var command = new RecordReceiptFromPurchaseCommand(
            integrationEvent.PurchaseId,
            integrationEvent.ReceivedOnUtc,
            lines);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to record goods receipt in Warehouse module: {result.Error}");
        }
    }
}
