using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.DeleteEmploymentStatusChangeApplication;

internal sealed class DeleteEmploymentStatusChangeApplicationCommandHandler
    : ICommandHandler<DeleteEmploymentStatusChangeApplicationCommand, DeleteEmploymentStatusChangeApplicationResponse>
{
    private readonly IEmploymentStatusChangeApplicationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmploymentStatusChangeApplicationCommandHandler(
        IEmploymentStatusChangeApplicationRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DeleteEmploymentStatusChangeApplicationResponse>> Handle(
        DeleteEmploymentStatusChangeApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure<DeleteEmploymentStatusChangeApplicationResponse>(
                EmploymentStatusChangeApplicationErrors.NotFound);

        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure<DeleteEmploymentStatusChangeApplicationResponse>(
                EmploymentStatusChangeApplicationErrors.AlreadyConverted);

        await _repository.DeleteAsync(application.Id, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteEmploymentStatusChangeApplicationResponse());
    }
}
