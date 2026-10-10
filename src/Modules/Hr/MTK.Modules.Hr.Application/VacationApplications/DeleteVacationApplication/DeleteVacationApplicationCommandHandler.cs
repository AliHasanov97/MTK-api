using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.VacationApplications;

namespace MTK.Modules.Hr.Application.VacationApplications.DeleteVacationApplication;

internal sealed class DeleteVacationApplicationCommandHandler
    : ICommandHandler<DeleteVacationApplicationCommand>
{
    private readonly IVacationApplicationRepository _vacationApplicationRepository;

    public DeleteVacationApplicationCommandHandler(IVacationApplicationRepository vacationApplicationRepository)
    {
        _vacationApplicationRepository = vacationApplicationRepository;
    }

    public async Task<Result> Handle(
        DeleteVacationApplicationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. VacationApplication tap
        var application = await _vacationApplicationRepository.GetByIdDefaultAsync(
            request.Id, cancellationToken);
        if (application is null)
            return Result.Failure(VacationApplicationErrors.NotFound);

        // 2. Əmrə çevrilmiş ərizələr silinə bilməz
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure(VacationApplicationErrors.AlreadyConvertedToOrder);

        // 3. Soft delete
        await _vacationApplicationRepository.DeleteAsync(application, null, cancellationToken);

        return Result.Success();
    }
}
