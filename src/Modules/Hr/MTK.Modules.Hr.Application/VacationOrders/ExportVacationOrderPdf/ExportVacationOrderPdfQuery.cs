using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationOrders.ExportVacationOrderPdf;

public sealed record ExportVacationOrderPdfQuery(Guid OrderId)
    : IQuery<ExportVacationOrderPdfResponse>;
