using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;

using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Jobs;

namespace MTK.Modules.Hr.Application.JobApplications.UpdateJobApplication;

internal sealed class UpdateJobApplicationCommandHandler : ICommandHandler<UpdateJobApplicationCommand, UpdateJobApplicationResponse>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateJobApplicationCommandHandler(
        IJobApplicationRepository jobApplicationRepository,
        IJobRepository jobRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UpdateJobApplicationResponse>> Handle(UpdateJobApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var jobApplication = await _jobApplicationRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
            if (jobApplication is null)
                return Result.Failure<UpdateJobApplicationResponse>(JobApplicationErrors.NotFound);

            // Əmrə çevrilmiş ərizələr yenilənə bilməz
            if (jobApplication.Status == ApplicationStatus.ConvertedToOrder)
                return Result.Failure<UpdateJobApplicationResponse>(JobApplicationErrors.AlreadyConverted);

            if (request.JobId.HasValue)
            {
                var job = await _jobRepository.GetByIdDefaultAsync(request.JobId.Value, cancellationToken);
                if (job is null)
                    return Result.Failure<UpdateJobApplicationResponse>(JobApplicationErrors.JobNotFound);
            }

            jobApplication.Update(
                request.Address,
                request.Name,
                request.Surname,
                request.FathersName,
                request.Telephone,
                request.HomeTelephoneNumber,
                request.Gender,
                request.JobId,
                request.StartDate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updated = await _jobApplicationRepository.GetByIdDefaultAsync(jobApplication.Id, cancellationToken);
            return Result.Success(_mapper.Map<UpdateJobApplicationResponse>(updated));
        }
        catch (NullReferenceException)
        {
            return Result.Failure<UpdateJobApplicationResponse>(JobApplicationErrors.NotFound);
        }
    }
}
