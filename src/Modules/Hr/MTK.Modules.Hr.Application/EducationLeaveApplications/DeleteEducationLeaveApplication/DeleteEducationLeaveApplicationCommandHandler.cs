using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.DeleteEducationLeaveApplication;

internal sealed class DeleteEducationLeaveApplicationCommandHandler
    : ICommandHandler<DeleteEducationLeaveApplicationCommand>
{
    private readonly IEducationLeaveApplicationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEducationLeaveApplicationCommandHandler(
        IEducationLeaveApplicationRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteEducationLeaveApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure(EducationLeaveApplicationErrors.NotFound);

        // Əmrə çevrilmiş ərizələr silinə bilməz
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure(EducationLeaveApplicationErrors.AlreadyConvertedToOrder);

        await _repository.DeleteAsync(application.Id, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
