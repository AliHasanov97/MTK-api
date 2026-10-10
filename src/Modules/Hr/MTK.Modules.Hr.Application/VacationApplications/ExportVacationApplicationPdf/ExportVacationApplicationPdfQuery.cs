using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationApplications.ExportVacationApplicationPdf;

public sealed record ExportVacationApplicationPdfQuery(Guid ApplicationId)
    : IQuery<ExportVacationApplicationPdfResponse>;