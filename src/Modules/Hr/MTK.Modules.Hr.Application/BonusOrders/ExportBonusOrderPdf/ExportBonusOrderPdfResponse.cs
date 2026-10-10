namespace MTK.Modules.Hr.Application.BonusOrders.ExportBonusOrderPdf;

public sealed record ExportBonusOrderPdfResponse(MemoryStream FileStream, string FileName);
