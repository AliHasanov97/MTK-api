using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.ExportUnpaidLeaveApplicationPdf;

public sealed record ExportUnpaidLeaveApplicationPdfQuery(Guid ApplicationId)
    : IQuery<ExportUnpaidLeaveApplicationPdfResponse>;