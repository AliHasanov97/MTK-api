namespace MTK.Modules.Hr.Application.VacationApplications.ExportVacationApplicationPdf;

public sealed record ExportVacationApplicationPdfResponse(
    Stream FileStream,
    string FileName);