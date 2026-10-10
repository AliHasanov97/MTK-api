using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.ExportEmploymentStatusChangeApplicationPdf;

internal sealed class ExportEmploymentStatusChangeApplicationPdfQueryHandler
    : IQueryHandler<ExportEmploymentStatusChangeApplicationPdfQuery, ExportEmploymentStatusChangeApplicationPdfResponse>
{
    private readonly IExportService _exportService;

    public ExportEmploymentStatusChangeApplicationPdfQueryHandler(IExportService exportService)
    {
        _exportService = exportService;
    }

    public async Task<Result<ExportEmploymentStatusChangeApplicationPdfResponse>> Handle(
        ExportEmploymentStatusChangeApplicationPdfQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _exportService.ExportEmploymentStatusChangeApplicationToPdfAsync(
                request.ApplicationId,
                cancellationToken);

            return Result.Success(new ExportEmploymentStatusChangeApplicationPdfResponse(
                (MemoryStream)result.FileStream,
                result.FileName));
        }
        catch (InvalidOperationException)
        {
            return Result.Failure<ExportEmploymentStatusChangeApplicationPdfResponse>(
                EmploymentStatusChangeApplicationErrors.NotFound);
        }
    }
}
