namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.ExportWorkOnNonWorkdayOrderPdf;

public sealed record ExportWorkOnNonWorkdayOrderPdfResponse(MemoryStream FileStream, string FileName);
