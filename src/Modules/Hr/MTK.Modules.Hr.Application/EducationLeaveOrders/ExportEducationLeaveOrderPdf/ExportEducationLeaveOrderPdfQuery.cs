using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EducationLeaveOrders.ExportEducationLeaveOrderPdf;

public sealed record ExportEducationLeaveOrderPdfQuery(Guid OrderId)
    : IQuery<ExportEducationLeaveOrderPdfResponse>;