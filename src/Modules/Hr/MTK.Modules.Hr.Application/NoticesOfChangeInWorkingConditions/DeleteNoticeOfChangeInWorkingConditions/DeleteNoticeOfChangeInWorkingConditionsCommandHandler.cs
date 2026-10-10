using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.NoticesOfChangeInWorkingConditions;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.DeleteNoticeOfChangeInWorkingConditions;

internal sealed class DeleteNoticeOfChangeInWorkingConditionsCommandHandler : ICommandHandler<DeleteNoticeOfChangeInWorkingConditionsCommand>
{
    private readonly INoticeOfChangeInWorkingConditionsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNoticeOfChangeInWorkingConditionsCommandHandler(
        INoticeOfChangeInWorkingConditionsRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteNoticeOfChangeInWorkingConditionsCommand request, CancellationToken cancellationToken)
    {
        var notice = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (notice is null)
            return Result.Failure(NoticeOfChangeInWorkingConditionsErrors.NotFound);

        await _repository.DeleteAsync(notice, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}