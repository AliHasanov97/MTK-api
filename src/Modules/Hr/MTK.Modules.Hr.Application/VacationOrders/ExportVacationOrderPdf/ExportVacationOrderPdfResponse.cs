namespace MTK.Modules.Hr.Application.VacationOrders.ExportVacationOrderPdf;

public sealed record ExportVacationOrderPdfResponse(
    Stream FileStream,
    string FileName);
