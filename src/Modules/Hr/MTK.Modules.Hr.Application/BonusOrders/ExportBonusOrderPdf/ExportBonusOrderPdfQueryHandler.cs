using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;

namespace MTK.Modules.Hr.Application.BonusOrders.ExportBonusOrderPdf;

internal sealed class ExportBonusOrderPdfQueryHandler : IQueryHandler<ExportBonusOrderPdfQuery, ExportBonusOrderPdfResponse>
{
    private readonly IExportService _exportService;

    public ExportBonusOrderPdfQueryHandler(IExportService exportService)
    {
        _exportService = exportService;
    }

    public async Task<Result<ExportBonusOrderPdfResponse>> Handle(ExportBonusOrderPdfQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _exportService.ExportBonusOrderToPdfAsync(request.BonusOrderId, cancellationToken);
            return Result.Success(new ExportBonusOrderPdfResponse((MemoryStream)result.FileStream, result.FileName));
        }
        catch (InvalidOperationException)
        {
            return Result.Failure<ExportBonusOrderPdfResponse>(BonusOrderErrors.NotFound);
        }
    }
}
