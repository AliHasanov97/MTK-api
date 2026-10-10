namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.ExportUnpaidLeaveApplicationPdf;

public sealed record ExportUnpaidLeaveApplicationPdfResponse(
    Stream FileStream,
    string FileName);