using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Purchases.Queries.ExportPurchase;

public sealed record ExportPurchaseQuery(Guid PurchaseId) : IQuery<ExportFileResult>;
