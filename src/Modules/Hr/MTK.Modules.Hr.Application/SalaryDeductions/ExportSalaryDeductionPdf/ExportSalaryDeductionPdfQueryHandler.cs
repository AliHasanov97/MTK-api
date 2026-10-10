using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;

namespace MTK.Modules.Hr.Application.SalaryDeductions.ExportSalaryDeductionPdf;

internal sealed class ExportSalaryDeductionPdfQueryHandler : IQueryHandler<ExportSalaryDeductionPdfQuery, ExportSalaryDeductionPdfResponse>
{
    private readonly IExportService _exportService;

    public ExportSalaryDeductionPdfQueryHandler(IExportService exportService)
    {
        _exportService = exportService;
    }

    public async Task<Result<ExportSalaryDeductionPdfResponse>> Handle(ExportSalaryDeductionPdfQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _exportService.ExportSalaryDeductionToPdfAsync(request.SalaryDeductionId, cancellationToken);
            return Result.Success(new ExportSalaryDeductionPdfResponse((MemoryStream)result.FileStream, result.FileName));
        }
        catch (InvalidOperationException)
        {
            return Result.Failure<ExportSalaryDeductionPdfResponse>(SalaryDeductionErrors.NotFound);
        }
    }
}
