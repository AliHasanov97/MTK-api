using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Jobs;

namespace MTK.Modules.Hr.Application.Jobs.GetJobById;

internal sealed class GetJobByIdQueryHandler : IQueryHandler<GetJobByIdQuery, GetJobByIdResponse>
{
    private readonly IJobRepository _jobRepository;
    private readonly IMapper _mapper;

    public GetJobByIdQueryHandler(IJobRepository jobRepository, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetJobByIdResponse>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (job is null)
            return Result.Failure<GetJobByIdResponse>(JobErrors.NotFound);

        return Result.Success(_mapper.Map<GetJobByIdResponse>(job));
    }
}
