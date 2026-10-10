using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.ExportVacationCompensationApplicationPdf;

public sealed record ExportVacationCompensationApplicationPdfQuery(Guid ApplicationId)
    : IQuery<ExportVacationCompensationApplicationPdfResponse>;
