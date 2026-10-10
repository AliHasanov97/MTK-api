namespace MTK.Modules.Hr.Application.EducationLeaveOrders.ExportEducationLeaveOrderPdf;

public sealed record ExportEducationLeaveOrderPdfResponse(
    Stream FileStream,
    string FileName);