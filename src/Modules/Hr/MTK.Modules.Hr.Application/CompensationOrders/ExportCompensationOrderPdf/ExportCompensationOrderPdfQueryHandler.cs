using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.CompensationOrders;

namespace MTK.Modules.Hr.Application.CompensationOrders.ExportCompensationOrderPdf;

internal sealed class ExportCompensationOrderPdfQueryHandler
    : IQueryHandler<ExportCompensationOrderPdfQuery, ExportCompensationOrderPdfResponse>
{
    private readonly ICompensationOrderRepository _repository;
    private readonly IExportService _exportService;

    public ExportCompensationOrderPdfQueryHandler(
        ICompensationOrderRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportCompensationOrderPdfResponse>> Handle(
        ExportCompensationOrderPdfQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<ExportCompensationOrderPdfResponse>(
                CompensationOrderErrors.NotFound(request.OrderId));
        }

        var exportResult = await _exportService.ExportCompensationOrderToPdfAsync(
            request.OrderId,
            cancellationToken);

        return Result.Success(new ExportCompensationOrderPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
