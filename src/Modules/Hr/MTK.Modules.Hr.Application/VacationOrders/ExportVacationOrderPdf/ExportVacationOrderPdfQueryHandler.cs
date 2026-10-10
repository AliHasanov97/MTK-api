using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.VacationOrders;

namespace MTK.Modules.Hr.Application.VacationOrders.ExportVacationOrderPdf;

internal sealed class ExportVacationOrderPdfQueryHandler
    : IQueryHandler<ExportVacationOrderPdfQuery, ExportVacationOrderPdfResponse>
{
    private readonly IVacationOrderRepository _repository;
    private readonly IExportService _exportService;

    public ExportVacationOrderPdfQueryHandler(
        IVacationOrderRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportVacationOrderPdfResponse>> Handle(
        ExportVacationOrderPdfQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<ExportVacationOrderPdfResponse>(
                new Error("VacationOrder.NotFound", "Əmək məzuniyyəti əmri tapılmadı"));
        }

        var exportResult = await _exportService.ExportVacationOrderToPdfAsync(
            request.OrderId,
            cancellationToken);

        return Result.Success(new ExportVacationOrderPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
