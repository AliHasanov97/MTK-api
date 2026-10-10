using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.ExportEmploymentStatusChangeOrderPdf;

internal sealed class ExportEmploymentStatusChangeOrderPdfQueryHandler
    : IQueryHandler<ExportEmploymentStatusChangeOrderPdfQuery, ExportEmploymentStatusChangeOrderPdfResponse>
{
    private readonly IExportService _exportService;

    public ExportEmploymentStatusChangeOrderPdfQueryHandler(IExportService exportService)
    {
        _exportService = exportService;
    }

    public async Task<Result<ExportEmploymentStatusChangeOrderPdfResponse>> Handle(
        ExportEmploymentStatusChangeOrderPdfQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _exportService.ExportEmploymentStatusChangeOrderToPdfAsync(
                request.OrderId,
                cancellationToken);

            return Result.Success(new ExportEmploymentStatusChangeOrderPdfResponse(
                (MemoryStream)result.FileStream,
                result.FileName));
        }
        catch (InvalidOperationException)
        {
            return Result.Failure<ExportEmploymentStatusChangeOrderPdfResponse>(
                EmploymentStatusChangeOrderErrors.NotFound);
        }
    }
}
