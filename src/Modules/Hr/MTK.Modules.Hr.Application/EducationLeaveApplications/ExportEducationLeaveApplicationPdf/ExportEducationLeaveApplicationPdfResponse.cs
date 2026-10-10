namespace MTK.Modules.Hr.Application.EducationLeaveApplications.ExportEducationLeaveApplicationPdf;

public sealed record ExportEducationLeaveApplicationPdfResponse(
    Stream FileStream,
    string FileName);