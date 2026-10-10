using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.ExportUnpaidLeaveApplicationPdf;

internal sealed class ExportUnpaidLeaveApplicationPdfQueryHandler
    : IQueryHandler<ExportUnpaidLeaveApplicationPdfQuery, ExportUnpaidLeaveApplicationPdfResponse>
{
    private readonly IUnpaidLeaveApplicationRepository _repository;
    private readonly IExportService _exportService;

    public ExportUnpaidLeaveApplicationPdfQueryHandler(
        IUnpaidLeaveApplicationRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportUnpaidLeaveApplicationPdfResponse>> Handle(
        ExportUnpaidLeaveApplicationPdfQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.ApplicationId, cancellationToken);

        if (application is null)
        {
            return Result.Failure<ExportUnpaidLeaveApplicationPdfResponse>(
                new Error("UnpaidLeaveApplication.NotFound", "Ödənişsiz məzuniyyət ərizəsi tapılmadı"));
        }

        var exportResult = await _exportService.ExportUnpaidLeaveApplicationToPdfAsync(
            request.ApplicationId,
            cancellationToken);

        return Result.Success(new ExportUnpaidLeaveApplicationPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
