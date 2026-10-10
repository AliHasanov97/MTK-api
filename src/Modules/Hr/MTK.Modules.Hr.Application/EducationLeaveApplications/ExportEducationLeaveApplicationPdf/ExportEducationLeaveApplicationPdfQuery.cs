using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.ExportEducationLeaveApplicationPdf;

public sealed record ExportEducationLeaveApplicationPdfQuery(Guid ApplicationId)
    : IQuery<ExportEducationLeaveApplicationPdfResponse>;