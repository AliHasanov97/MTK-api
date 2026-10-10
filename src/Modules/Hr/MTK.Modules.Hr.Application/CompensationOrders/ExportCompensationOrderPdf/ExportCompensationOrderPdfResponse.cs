namespace MTK.Modules.Hr.Application.CompensationOrders.ExportCompensationOrderPdf;

public sealed record ExportCompensationOrderPdfResponse(
    Stream FileStream,
    string FileName);
