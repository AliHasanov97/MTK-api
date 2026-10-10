using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.ExportUnexcusedAbsencePdf;

public sealed record ExportUnexcusedAbsencePdfQuery(Guid UnexcusedAbsenceId)
    : IQuery<ExportUnexcusedAbsencePdfResponse>;
