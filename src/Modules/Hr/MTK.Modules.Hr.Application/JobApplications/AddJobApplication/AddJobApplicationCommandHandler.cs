using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Jobs;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Jobs;

namespace MTK.Modules.Hr.Application.JobApplications.AddJobApplication;

internal sealed class AddJobApplicationCommandHandler : ICommandHandler<AddJobApplicationCommand, AddJobApplicationResponse>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public AddJobApplicationCommandHandler(
        IJobApplicationRepository jobApplicationRepository,
        IJobRepository jobRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<AddJobApplicationResponse>> Handle(AddJobApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var job = await _jobRepository.GetByIdDefaultAsync(request.JobId, cancellationToken);
            if (job is null)
                return Result.Failure<AddJobApplicationResponse>(JobApplicationErrors.JobNotFound);

            var jobApplication = JobApplication.Create(
                request.Address,
                request.Name,
                request.Surname,
                request.FathersName,
                request.Telephone,
                request.HomeTelephoneNumber,
                request.Gender,
                request.JobId,
                request.StartDate,
                _userContext.UserId);

            await _jobApplicationRepository.AddAsync(jobApplication, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var saved = await _jobApplicationRepository.GetByIdDefaultAsync(jobApplication.Id, cancellationToken);
            return Result.Success(_mapper.Map<AddJobApplicationResponse>(saved));
        }
        catch (NullReferenceException)
        {
            return Result.Failure<AddJobApplicationResponse>(JobApplicationErrors.NotFound);
        }
        catch (Exception)
        {
            return Result.Failure<AddJobApplicationResponse>(
                JobApplicationErrors.AddFailed);
        }
    }
}
