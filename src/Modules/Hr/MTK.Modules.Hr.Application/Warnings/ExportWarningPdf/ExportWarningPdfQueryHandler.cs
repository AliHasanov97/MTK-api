using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Application.Warnings.ExportWarningPdf;

internal sealed class ExportWarningPdfQueryHandler
    : IQueryHandler<ExportWarningPdfQuery, ExportWarningPdfResponse>
{
    private readonly IWarningRepository _repository;
    private readonly IExportService _exportService;

    public ExportWarningPdfQueryHandler(
        IWarningRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportWarningPdfResponse>> Handle(
        ExportWarningPdfQuery request,
        CancellationToken cancellationToken)
    {
        var warning = await _repository.GetByIdDefaultAsync(request.WarningId, cancellationToken);

        if (warning is null)
        {
            return Result.Failure<ExportWarningPdfResponse>(WarningErrors.NotFound);
        }

        var exportResult = await _exportService.ExportWarningToPdfAsync(
            request.WarningId,
            cancellationToken);

        return Result.Success(new ExportWarningPdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
