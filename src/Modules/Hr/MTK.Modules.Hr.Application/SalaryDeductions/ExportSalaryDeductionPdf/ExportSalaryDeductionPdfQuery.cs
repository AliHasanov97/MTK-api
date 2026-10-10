using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.SalaryDeductions.ExportSalaryDeductionPdf;

public sealed record ExportSalaryDeductionPdfQuery(Guid SalaryDeductionId) : IQuery<ExportSalaryDeductionPdfResponse>;
