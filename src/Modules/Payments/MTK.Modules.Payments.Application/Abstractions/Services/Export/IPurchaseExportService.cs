using MTK.Modules.Payments.Application.Purchases.Queries.GetPurchaseById;

namespace MTK.Modules.Payments.Application.Abstractions.Services.Export;

public interface IPurchaseExportService
{
    MemoryStream ExportToPdf(PurchaseResponse purchase);
}
