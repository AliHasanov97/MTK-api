namespace MTK.Modules.Hr.Application.SalaryDeductions.ExportSalaryDeductionPdf;

public sealed record ExportSalaryDeductionPdfResponse(MemoryStream FileStream, string FileName);
