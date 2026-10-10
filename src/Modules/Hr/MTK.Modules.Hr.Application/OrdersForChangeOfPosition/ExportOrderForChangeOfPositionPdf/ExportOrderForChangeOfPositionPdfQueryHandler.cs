using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;

namespace MTK.Modules.Hr.Application.OrdersForChangeOfPosition.ExportOrderForChangeOfPositionPdf;

internal sealed class ExportOrderForChangeOfPositionPdfQueryHandler
    : IQueryHandler<ExportOrderForChangeOfPositionPdfQuery, ExportOrderForChangeOfPositionPdfResponse>
{
    private readonly IOrderForChangeOfPositionRepository _repository;
    private readonly IExportService _exportService;

    public ExportOrderForChangeOfPositionPdfQueryHandler(
        IOrderForChangeOfPositionRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportOrderForChangeOfPositionPdfResponse>> Handle(
        ExportOrderForChangeOfPositionPdfQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<ExportOrderForChangeOfPositionPdfResponse>(
                OrderForChangeOfPositionErrors.NotFound);
        }

        var exportResult = await _exportService.ExportOrderForChangeOfPositionToPdfAsync(
            request.OrderId,
            cancellationToken);

        return Result.Success(new ExportOrderForChangeOfPositionPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
