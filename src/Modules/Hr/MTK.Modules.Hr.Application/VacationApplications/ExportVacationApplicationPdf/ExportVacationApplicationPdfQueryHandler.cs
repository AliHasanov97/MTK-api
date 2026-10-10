using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.VacationApplications;

namespace MTK.Modules.Hr.Application.VacationApplications.ExportVacationApplicationPdf;

internal sealed class ExportVacationApplicationPdfQueryHandler
    : IQueryHandler<ExportVacationApplicationPdfQuery, ExportVacationApplicationPdfResponse>
{
    private readonly IVacationApplicationRepository _repository;
    private readonly IExportService _exportService;

    public ExportVacationApplicationPdfQueryHandler(
        IVacationApplicationRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportVacationApplicationPdfResponse>> Handle(
        ExportVacationApplicationPdfQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.ApplicationId, cancellationToken);

        if (application is null)
        {
            return Result.Failure<ExportVacationApplicationPdfResponse>(
                VacationApplicationErrors.NotFound);
        }

        var exportResult = await _exportService.ExportVacationApplicationToPdfAsync(
            request.ApplicationId,
            cancellationToken);

        return Result.Success(new ExportVacationApplicationPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}