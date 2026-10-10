using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.CompensationOrders.ExportCompensationOrderPdf;

public sealed record ExportCompensationOrderPdfQuery(Guid OrderId)
    : IQuery<ExportCompensationOrderPdfResponse>;
