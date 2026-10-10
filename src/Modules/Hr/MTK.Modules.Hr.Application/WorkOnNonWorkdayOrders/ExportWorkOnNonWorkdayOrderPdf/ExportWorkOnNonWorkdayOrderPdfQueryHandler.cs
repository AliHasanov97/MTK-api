using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.ExportWorkOnNonWorkdayOrderPdf;

internal sealed class ExportWorkOnNonWorkdayOrderPdfQueryHandler : IQueryHandler<ExportWorkOnNonWorkdayOrderPdfQuery, ExportWorkOnNonWorkdayOrderPdfResponse>
{
    private readonly IExportService _exportService;

    public ExportWorkOnNonWorkdayOrderPdfQueryHandler(IExportService exportService)
    {
        _exportService = exportService;
    }

    public async Task<Result<ExportWorkOnNonWorkdayOrderPdfResponse>> Handle(ExportWorkOnNonWorkdayOrderPdfQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _exportService.ExportWorkOnNonWorkdayOrderToPdfAsync(request.WorkOnNonWorkdayOrderId, cancellationToken);
            return Result.Success(new ExportWorkOnNonWorkdayOrderPdfResponse((MemoryStream)result.FileStream, result.FileName));
        }
        catch (InvalidOperationException)
        {
            return Result.Failure<ExportWorkOnNonWorkdayOrderPdfResponse>(WorkOnNonWorkdayOrderErrors.NotFound);
        }
    }
}
