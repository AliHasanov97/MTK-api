using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.ExportWorkOnNonWorkdayOrderPdf;

public sealed record ExportWorkOnNonWorkdayOrderPdfQuery(Guid WorkOnNonWorkdayOrderId) : IQuery<ExportWorkOnNonWorkdayOrderPdfResponse>;
