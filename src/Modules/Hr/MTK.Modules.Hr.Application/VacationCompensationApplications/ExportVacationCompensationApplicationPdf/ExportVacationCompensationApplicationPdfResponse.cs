namespace MTK.Modules.Hr.Application.VacationCompensationApplications.ExportVacationCompensationApplicationPdf;

public sealed record ExportVacationCompensationApplicationPdfResponse(
    Stream FileStream,
    string FileName);
