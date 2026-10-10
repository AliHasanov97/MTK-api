namespace MTK.Modules.Hr.Application.Abstractions.Services.ExportService;

public sealed record ExportResult(Stream FileStream, string FileName);
