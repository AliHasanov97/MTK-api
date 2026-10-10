using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Jobs;

namespace MTK.Modules.Hr.Application.Jobs.SearchJobs;

internal sealed class SearchJobsQueryHandler : IQueryHandler<SearchJobsQuery, SearchJobsResponse>
{
    private readonly IJobRepository _jobRepository;
    private readonly IMapper _mapper;

    public SearchJobsQueryHandler(IJobRepository jobRepository, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchJobsResponse>> Handle(SearchJobsQuery request, CancellationToken cancellationToken)
    {
        var jobs = await _jobRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _jobRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var data = _mapper.Map<List<SearchJobsResponseItem>>(jobs);
        return Result.Success(new SearchJobsResponse(data, totalCount, request.Page, request.PageSize));
    }
}
