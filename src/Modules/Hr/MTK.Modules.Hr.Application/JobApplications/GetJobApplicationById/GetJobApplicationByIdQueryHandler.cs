using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Application.JobApplications.GetJobApplicationById;

internal sealed class GetJobApplicationByIdQueryHandler : IQueryHandler<GetJobApplicationByIdQuery, GetJobApplicationByIdResponse>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IMapper _mapper;

    public GetJobApplicationByIdQueryHandler(IJobApplicationRepository jobApplicationRepository, IMapper mapper)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetJobApplicationByIdResponse>> Handle(GetJobApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var jobApplication = await _jobApplicationRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (jobApplication is null)
            return Result.Failure<GetJobApplicationByIdResponse>(JobApplicationErrors.NotFound);

        return Result.Success(_mapper.Map<GetJobApplicationByIdResponse>(jobApplication));
    }
}
