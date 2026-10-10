using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.DeleteUnpaidLeaveApplication;

internal sealed class DeleteUnpaidLeaveApplicationCommandHandler
    : ICommandHandler<DeleteUnpaidLeaveApplicationCommand>
{
    private readonly IUnpaidLeaveApplicationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUnpaidLeaveApplicationCommandHandler(
        IUnpaidLeaveApplicationRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteUnpaidLeaveApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure(UnpaidLeaveApplicationErrors.NotFound);

        // Əmrə çevrilmiş ərizələr silinə bilməz
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure(UnpaidLeaveApplicationErrors.AlreadyConvertedToOrder);

        await _repository.DeleteAsync(application.Id, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
