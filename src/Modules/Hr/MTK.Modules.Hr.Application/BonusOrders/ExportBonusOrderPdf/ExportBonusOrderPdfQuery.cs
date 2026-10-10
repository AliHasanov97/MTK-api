using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.BonusOrders.ExportBonusOrderPdf;

public sealed record ExportBonusOrderPdfQuery(Guid BonusOrderId) : IQuery<ExportBonusOrderPdfResponse>;
