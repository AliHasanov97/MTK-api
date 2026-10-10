using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.ExportEducationLeaveApplicationPdf;

internal sealed class ExportEducationLeaveApplicationPdfQueryHandler
    : IQueryHandler<ExportEducationLeaveApplicationPdfQuery, ExportEducationLeaveApplicationPdfResponse>
{
    private readonly IEducationLeaveApplicationRepository _repository;
    private readonly IExportService _exportService;

    public ExportEducationLeaveApplicationPdfQueryHandler(
        IEducationLeaveApplicationRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportEducationLeaveApplicationPdfResponse>> Handle(
        ExportEducationLeaveApplicationPdfQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.ApplicationId, cancellationToken);

        if (application is null)
        {
            return Result.Failure<ExportEducationLeaveApplicationPdfResponse>(
                new Error("EducationLeaveApplication.NotFound", "Təhsil məzuniyyəti ərizəsi tapılmadı"));
        }

        var exportResult = await _exportService.ExportEducationLeaveApplicationToPdfAsync(
            request.ApplicationId,
            cancellationToken);

        return Result.Success(new ExportEducationLeaveApplicationPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
