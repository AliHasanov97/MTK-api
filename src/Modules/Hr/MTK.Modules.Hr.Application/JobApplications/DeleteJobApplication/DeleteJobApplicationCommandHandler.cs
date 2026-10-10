using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.JobApplications;
using MediatR;

namespace MTK.Modules.Hr.Application.JobApplications.DeleteJobApplication;

internal sealed class DeleteJobApplicationCommandHandler : ICommandHandler<DeleteJobApplicationCommand, Unit>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteJobApplicationCommandHandler(
        IJobApplicationRepository jobApplicationRepository,
        IUnitOfWork unitOfWork)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(DeleteJobApplicationCommand request, CancellationToken cancellationToken)
    {
        var jobApplication = await _jobApplicationRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (jobApplication is null)
            return Result.Failure<Unit>(JobApplicationErrors.NotFound);

        if (jobApplication.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure<Unit>(JobApplicationErrors.AlreadyConverted);

        await _jobApplicationRepository.DeleteAsync(jobApplication, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
