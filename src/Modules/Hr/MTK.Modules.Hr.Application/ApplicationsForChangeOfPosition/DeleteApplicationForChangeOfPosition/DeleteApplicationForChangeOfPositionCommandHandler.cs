using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.DeleteApplicationForChangeOfPosition;

internal sealed class DeleteApplicationForChangeOfPositionCommandHandler
    : ICommandHandler<DeleteApplicationForChangeOfPositionCommand>
{
    private readonly IApplicationForChangeOfPositionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteApplicationForChangeOfPositionCommandHandler(
        IApplicationForChangeOfPositionRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteApplicationForChangeOfPositionCommand request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (application is null)
            return Result.Failure(ApplicationForChangeOfPositionErrors.NotFound);

        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure(ApplicationForChangeOfPositionErrors.AlreadyConverted);

        await _repository.DeleteAsync(application, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
