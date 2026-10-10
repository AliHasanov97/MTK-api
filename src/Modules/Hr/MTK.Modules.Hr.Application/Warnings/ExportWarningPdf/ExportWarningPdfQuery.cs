using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.Warnings.ExportWarningPdf;

public sealed record ExportWarningPdfQuery(Guid WarningId)
    : IQuery<ExportWarningPdfResponse>;
