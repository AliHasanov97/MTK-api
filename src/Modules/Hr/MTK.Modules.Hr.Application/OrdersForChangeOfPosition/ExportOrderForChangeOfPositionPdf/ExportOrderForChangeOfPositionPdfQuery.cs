using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.OrdersForChangeOfPosition.ExportOrderForChangeOfPositionPdf;

public sealed record ExportOrderForChangeOfPositionPdfQuery(Guid OrderId)
    : IQuery<ExportOrderForChangeOfPositionPdfResponse>;
