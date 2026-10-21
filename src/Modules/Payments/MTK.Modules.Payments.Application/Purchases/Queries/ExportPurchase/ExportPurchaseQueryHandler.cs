using MediatR;
using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Purchases.Queries.GetPurchaseById;

namespace MTK.Modules.Payments.Application.Purchases.Queries.ExportPurchase;

internal sealed class ExportPurchaseQueryHandler : IQueryHandler<ExportPurchaseQuery, ExportFileResult>
{
    private readonly ISender _sender;
    private readonly IPurchaseExportService _exportService;

    public ExportPurchaseQueryHandler(ISender sender, IPurchaseExportService exportService)
    {
        _sender = sender;
        _exportService = exportService;
    }

    public async Task<Result<ExportFileResult>> Handle(
        ExportPurchaseQuery request,
        CancellationToken cancellationToken)
    {
        var purchaseResult = await _sender.Send(
            new GetPurchaseByIdQuery(request.PurchaseId), cancellationToken);

        if (purchaseResult.IsFailure)
            return Result.Failure<ExportFileResult>(purchaseResult.Error);

        var purchase = purchaseResult.Value;
        var stream = _exportService.ExportToPdf(purchase);
        var reference = string.IsNullOrWhiteSpace(purchase.InvoiceNumber)
            ? purchase.Id.ToString("N")[..8].ToUpperInvariant()
            : purchase.InvoiceNumber;
        var fileName = $"Satinalma_{reference}.pdf";

        return new ExportFileResult(stream.ToArray(), fileName, "application/pdf");
    }
}
