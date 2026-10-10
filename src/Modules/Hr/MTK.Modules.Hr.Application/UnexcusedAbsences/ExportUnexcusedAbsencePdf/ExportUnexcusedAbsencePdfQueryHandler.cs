using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Application.UnexcusedAbsences;
using MTK.Modules.Hr.Domain.UnexcusedAbsences;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.ExportUnexcusedAbsencePdf;

internal sealed class ExportUnexcusedAbsencePdfQueryHandler
    : IQueryHandler<ExportUnexcusedAbsencePdfQuery, ExportUnexcusedAbsencePdfResponse>
{
    private readonly IUnexcusedAbsenceRepository _repository;
    private readonly IExportService _exportService;

    public ExportUnexcusedAbsencePdfQueryHandler(
        IUnexcusedAbsenceRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportUnexcusedAbsencePdfResponse>> Handle(
        ExportUnexcusedAbsencePdfQuery request,
        CancellationToken cancellationToken)
    {
        var absence = await _repository.GetByIdDefaultAsync(request.UnexcusedAbsenceId, cancellationToken);

        if (absence is null)
        {
            return Result.Failure<ExportUnexcusedAbsencePdfResponse>(UnexcusedAbsenceErrors.NotFound);
        }

        var exportResult = await _exportService.ExportUnexcusedAbsenceToPdfAsync(
            request.UnexcusedAbsenceId,
            cancellationToken);

        return Result.Success(new ExportUnexcusedAbsencePdfResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
