using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Jobs;

namespace MTK.Modules.Hr.Application.Jobs.DeleteJob;

internal sealed class DeleteJobCommandHandler : ICommandHandler<DeleteJobCommand>
{
    private readonly IJobRepository _jobRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IApplicationForChangeOfPositionRepository _changeOfPositionRepository;

    public DeleteJobCommandHandler(
        IJobRepository jobRepository,
        IEmployeeRepository employeeRepository,
        IJobApplicationRepository jobApplicationRepository,
        IApplicationForChangeOfPositionRepository changeOfPositionRepository)
    {
        _jobRepository = jobRepository;
        _employeeRepository = employeeRepository;
        _jobApplicationRepository = jobApplicationRepository;
        _changeOfPositionRepository = changeOfPositionRepository;
    }

    public async Task<Result> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
    {
        // Silmə "soft"dur və FK pozuntusu yaratmır, ona görə istifadədə olan vəzifə əvvəlcədən yoxlanılır.
        bool inUse =
            (await _employeeRepository.ListAsync(e => e.JobId == request.JobId, cancellationToken)).Count > 0
            || (await _jobApplicationRepository.ListAsync(a => a.JobId == request.JobId, cancellationToken)).Count > 0
            || (await _changeOfPositionRepository.ListAsync(
                a => a.CurrentJobId == request.JobId || a.NewJobId == request.JobId, cancellationToken)).Count > 0;

        if (inUse)
        {
            return Result.Failure(JobErrors.InUse);
        }

        await _jobRepository.DeleteAsync(request.JobId, null, cancellationToken);
        return Result.Success();
    }
}
