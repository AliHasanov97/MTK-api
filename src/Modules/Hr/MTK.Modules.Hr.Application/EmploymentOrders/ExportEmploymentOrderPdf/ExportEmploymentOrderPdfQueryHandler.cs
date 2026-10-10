using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.EmploymentOrders;

namespace MTK.Modules.Hr.Application.EmploymentOrders.ExportEmploymentOrderPdf;

internal sealed class ExportEmploymentOrderPdfQueryHandler
    : IQueryHandler<ExportEmploymentOrderPdfQuery, ExportEmploymentOrderPdfResponse>
{
    private readonly IEmploymentOrderRepository _repository;
    private readonly IExportService _exportService;

    public ExportEmploymentOrderPdfQueryHandler(
        IEmploymentOrderRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportEmploymentOrderPdfResponse>> Handle(
        ExportEmploymentOrderPdfQuery request,
        CancellationToken cancellationToken)
    {
        var employmentOrder = await _repository.GetByIdDefaultAsync(
            request.EmploymentOrderId,
            cancellationToken);

        if (employmentOrder is null)
        {
            return Result.Failure<ExportEmploymentOrderPdfResponse>(
                EmploymentOrderErrors.NotFound);
        }

        var exportResult = await _exportService.ExportEmploymentOrderToPdfAsync(
            employmentOrder.Id,
            cancellationToken);

        return Result.Success(new ExportEmploymentOrderPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
