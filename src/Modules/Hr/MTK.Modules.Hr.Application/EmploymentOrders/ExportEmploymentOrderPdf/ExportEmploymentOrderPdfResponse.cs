namespace MTK.Modules.Hr.Application.EmploymentOrders.ExportEmploymentOrderPdf;

public sealed record ExportEmploymentOrderPdfResponse(Stream FileStream, string FileName);
