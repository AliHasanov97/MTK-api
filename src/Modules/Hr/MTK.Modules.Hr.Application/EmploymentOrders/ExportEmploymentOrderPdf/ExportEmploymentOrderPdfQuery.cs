using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmploymentOrders.ExportEmploymentOrderPdf;

public sealed record ExportEmploymentOrderPdfQuery(Guid EmploymentOrderId) : IQuery<ExportEmploymentOrderPdfResponse>;
