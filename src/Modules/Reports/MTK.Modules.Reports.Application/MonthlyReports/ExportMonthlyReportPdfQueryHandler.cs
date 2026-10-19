using MediatR;
using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Reports.Application.Abstractions;

namespace MTK.Modules.Reports.Application.MonthlyReports;

internal sealed class ExportMonthlyReportPdfQueryHandler
    : IQueryHandler<ExportMonthlyReportPdfQuery, ExportFileResult>
{
    private const string PdfContentType = "application/pdf";

    private readonly ISender _sender;
    private readonly IMonthlyReportPdfExportService _pdfExportService;

    public ExportMonthlyReportPdfQueryHandler(
        ISender sender,
        IMonthlyReportPdfExportService pdfExportService)
    {
        _sender = sender;
        _pdfExportService = pdfExportService;
    }

    public async Task<Result<ExportFileResult>> Handle(
        ExportMonthlyReportPdfQuery request,
        CancellationToken cancellationToken)
    {
        var reportResult = await _sender.Send(
            new GetMonthlyReportQuery(request.Year, request.Month),
            cancellationToken);

        if (reportResult.IsFailure)
        {
            return Result.Failure<ExportFileResult>(reportResult.Error);
        }

        var stream = _pdfExportService.ExportToPdf(reportResult.Value);
        var fileName = $"Ayliq_Hesabat_{request.Year}_{request.Month:00}.pdf";

        return new ExportFileResult(stream.ToArray(), fileName, PdfContentType);
    }
}
