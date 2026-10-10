using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;

namespace MTK.Modules.Hr.Application.UnpaidLeaveOrders.ExportUnpaidLeaveOrderPdf;

internal sealed class ExportUnpaidLeaveOrderPdfQueryHandler
    : IQueryHandler<ExportUnpaidLeaveOrderPdfQuery, ExportUnpaidLeaveOrderPdfResponse>
{
    private readonly IUnpaidLeaveOrderRepository _repository;
    private readonly IExportService _exportService;

    public ExportUnpaidLeaveOrderPdfQueryHandler(
        IUnpaidLeaveOrderRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportUnpaidLeaveOrderPdfResponse>> Handle(
        ExportUnpaidLeaveOrderPdfQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<ExportUnpaidLeaveOrderPdfResponse>(
                new Error("UnpaidLeaveOrder.NotFound", "Ödənişsiz məzuniyyət əmri tapılmadı"));
        }

        var exportResult = await _exportService.ExportUnpaidLeaveOrderToPdfAsync(
            request.OrderId,
            cancellationToken);

        return Result.Success(new ExportUnpaidLeaveOrderPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}