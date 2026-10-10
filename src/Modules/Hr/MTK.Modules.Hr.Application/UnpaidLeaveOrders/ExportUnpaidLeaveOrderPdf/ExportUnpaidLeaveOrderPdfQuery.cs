using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnpaidLeaveOrders.ExportUnpaidLeaveOrderPdf;

public sealed record ExportUnpaidLeaveOrderPdfQuery(Guid OrderId)
    : IQuery<ExportUnpaidLeaveOrderPdfResponse>;