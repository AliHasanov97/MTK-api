using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.VacationReturnApplications;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.DeleteVacationReturnApplication;

internal sealed class DeleteVacationReturnApplicationCommandHandler
    : ICommandHandler<DeleteVacationReturnApplicationCommand>
{
    private readonly IVacationReturnApplicationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVacationReturnApplicationCommandHandler(
        IVacationReturnApplicationRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteVacationReturnApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure(VacationReturnApplicationErrors.NotFound);

        // Əmrə çevrilmiş ərizələr silinə bilməz
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure(VacationReturnApplicationErrors.CannotDeleteNonPendingApplication);

        await _repository.DeleteAsync(application.Id, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}