using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Domain.NoticesOfChangeInWorkingConditions;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.ExportNoticeOfChangeInWorkingConditionsWord;

internal sealed class ExportNoticeOfChangeInWorkingConditionsWordQueryHandler
    : IQueryHandler<ExportNoticeOfChangeInWorkingConditionsWordQuery, ExportNoticeOfChangeInWorkingConditionsWordResponse>
{
    private readonly INoticeOfChangeInWorkingConditionsRepository _repository;
    private readonly IExportService _exportService;

    public ExportNoticeOfChangeInWorkingConditionsWordQueryHandler(
        INoticeOfChangeInWorkingConditionsRepository repository,
        IExportService exportService)
    {
        _repository = repository;
        _exportService = exportService;
    }

    public async Task<Result<ExportNoticeOfChangeInWorkingConditionsWordResponse>> Handle(
        ExportNoticeOfChangeInWorkingConditionsWordQuery request,
        CancellationToken cancellationToken)
    {
        var notice = await _repository.GetByIdDefaultAsync(request.NoticeId, cancellationToken);

        if (notice is null)
        {
            return Result.Failure<ExportNoticeOfChangeInWorkingConditionsWordResponse>(
                NoticeOfChangeInWorkingConditionsErrors.NotFound);
        }

        var exportResult = await _exportService.ExportNoticeOfChangeInWorkingConditionsToWordAsync(
            request.NoticeId,
            cancellationToken);

        return Result.Success(new ExportNoticeOfChangeInWorkingConditionsWordResponse(
            exportResult.FileStream,
            exportResult.FileName));
    }
}
