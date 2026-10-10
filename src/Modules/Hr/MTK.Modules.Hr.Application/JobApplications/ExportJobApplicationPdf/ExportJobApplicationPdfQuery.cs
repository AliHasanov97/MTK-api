using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.JobApplications.ExportJobApplicationPdf;

public sealed record ExportJobApplicationPdfQuery(Guid JobApplicationId) : IQuery<ExportJobApplicationPdfResponse>;
