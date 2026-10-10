using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.ExportApplicationForChangeOfPositionPdf;

internal sealed class ExportApplicationForChangeOfPositionPdfQueryHandler
    : IQueryHandler<ExportApplicationForChangeOfPositionPdfQuery, ExportApplicationForChangeOfPositionPdfResponse>
{
    private readonly IApplicationForChangeOfPositionRepository _repository;
    private readonly IExportService _exportService;

    public ExportApplicationForChangeOfPositionPdfQueryHandler(
        IApplicationForChangeOfPositionRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportApplicationForChangeOfPositionPdfResponse>> Handle(
        ExportApplicationForChangeOfPositionPdfQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.ApplicationId, cancellationToken);

        if (application is null)
        {
            return Result.Failure<ExportApplicationForChangeOfPositionPdfResponse>(
                ApplicationForChangeOfPositionErrors.NotFound);
        }

        var exportResult = await _exportService.ExportApplicationForChangeOfPositionToPdfAsync(
            request.ApplicationId,
            cancellationToken);

        return Result.Success(new ExportApplicationForChangeOfPositionPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
