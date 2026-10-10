using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Application.JobApplications.ExportJobApplicationPdf;

internal sealed class ExportJobApplicationPdfQueryHandler
    : IQueryHandler<ExportJobApplicationPdfQuery, ExportJobApplicationPdfResponse>
{
    private readonly IJobApplicationRepository _repository;
    private readonly IExportService _exportService;

    public ExportJobApplicationPdfQueryHandler(
        IJobApplicationRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportJobApplicationPdfResponse>> Handle(
        ExportJobApplicationPdfQuery request,
        CancellationToken cancellationToken)
    {
        var jobApplication = await _repository.GetByIdDefaultAsync(
            request.JobApplicationId,
            cancellationToken);

        if (jobApplication == null)
        {
            return Result.Failure<ExportJobApplicationPdfResponse>(
                JobApplicationErrors.NotFound);
        }

        var exportResult = await _exportService.ExportJobApplicationToPdfAsync(
            jobApplication.Id,
            cancellationToken);

        return Result.Success(new ExportJobApplicationPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
