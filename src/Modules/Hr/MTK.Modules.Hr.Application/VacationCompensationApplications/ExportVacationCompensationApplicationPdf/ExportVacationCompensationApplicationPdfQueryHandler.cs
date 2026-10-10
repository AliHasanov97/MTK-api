using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.ExportVacationCompensationApplicationPdf;

internal sealed class ExportVacationCompensationApplicationPdfQueryHandler
    : IQueryHandler<ExportVacationCompensationApplicationPdfQuery, ExportVacationCompensationApplicationPdfResponse>
{
    private readonly IVacationCompensationApplicationRepository _repository;
    private readonly IExportService _exportService;

    public ExportVacationCompensationApplicationPdfQueryHandler(
        IVacationCompensationApplicationRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportVacationCompensationApplicationPdfResponse>> Handle(
        ExportVacationCompensationApplicationPdfQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.ApplicationId, cancellationToken);

        if (application is null)
        {
            return Result.Failure<ExportVacationCompensationApplicationPdfResponse>(
                VacationCompensationApplicationErrors.NotFound);
        }

        var exportResult = await _exportService.ExportVacationCompensationApplicationToPdfAsync(
            request.ApplicationId,
            cancellationToken);

        return Result.Success(new ExportVacationCompensationApplicationPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
