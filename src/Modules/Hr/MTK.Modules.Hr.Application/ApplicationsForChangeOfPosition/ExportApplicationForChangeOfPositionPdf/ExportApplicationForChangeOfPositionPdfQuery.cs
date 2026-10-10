using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.ExportApplicationForChangeOfPositionPdf;

public sealed record ExportApplicationForChangeOfPositionPdfQuery(Guid ApplicationId)
    : IQuery<ExportApplicationForChangeOfPositionPdfResponse>;
