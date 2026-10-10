using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.ExportEmploymentStatusChangeOrderPdf;

public sealed record ExportEmploymentStatusChangeOrderPdfQuery(Guid OrderId) : IQuery<ExportEmploymentStatusChangeOrderPdfResponse>;
