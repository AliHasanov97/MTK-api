namespace MTK.Modules.Hr.Application.UnpaidLeaveOrders.ExportUnpaidLeaveOrderPdf;

public sealed record ExportUnpaidLeaveOrderPdfResponse(
    Stream FileStream,
    string FileName);