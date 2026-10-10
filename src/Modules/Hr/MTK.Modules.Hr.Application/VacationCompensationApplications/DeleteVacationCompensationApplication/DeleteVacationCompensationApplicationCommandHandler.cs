using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.DeleteVacationCompensationApplication;

internal sealed class DeleteVacationCompensationApplicationCommandHandler
    : ICommandHandler<DeleteVacationCompensationApplicationCommand>
{
    private readonly IVacationCompensationApplicationRepository _repository;

    public DeleteVacationCompensationApplicationCommandHandler(IVacationCompensationApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        DeleteVacationCompensationApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdWithLinesAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure(VacationCompensationApplicationErrors.NotFound);

        // Əmrə çevrilmiş ərizələr silinə bilməz
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure(VacationCompensationApplicationErrors.AlreadyConvertedToOrder);

        await _repository.DeleteAsync(application.Id, null, cancellationToken);

        return Result.Success();
    }
}
