using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;

namespace MTK.Modules.Hr.Application.EducationLeaveOrders.ExportEducationLeaveOrderPdf;

internal sealed class ExportEducationLeaveOrderPdfQueryHandler
    : IQueryHandler<ExportEducationLeaveOrderPdfQuery, ExportEducationLeaveOrderPdfResponse>
{
    private readonly IEducationLeaveOrderRepository _repository;
    private readonly IExportService _exportService;

    public ExportEducationLeaveOrderPdfQueryHandler(
        IEducationLeaveOrderRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportEducationLeaveOrderPdfResponse>> Handle(
        ExportEducationLeaveOrderPdfQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<ExportEducationLeaveOrderPdfResponse>(
                new Error("EducationLeaveOrder.NotFound", "Təhsil məzuniyyəti əmri tapılmadı"));
        }

        var exportResult = await _exportService.ExportEducationLeaveOrderToPdfAsync(
            request.OrderId,
            cancellationToken);

        return Result.Success(new ExportEducationLeaveOrderPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}