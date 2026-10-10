using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Application.JobApplications.SearchJobApplications;

internal sealed class SearchJobApplicationsQueryHandler : IQueryHandler<SearchJobApplicationsQuery, SearchJobApplicationsResponse>
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IMapper _mapper;

    public SearchJobApplicationsQueryHandler(
        IJobApplicationRepository jobApplicationRepository,
        IMapper mapper)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchJobApplicationsResponse>> Handle(SearchJobApplicationsQuery request, CancellationToken cancellationToken)
    {
        var jobApplications = await _jobApplicationRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _jobApplicationRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var data = _mapper.Map<List<SearchJobApplicationsResponseItem>>(jobApplications);
        return Result.Success(new SearchJobApplicationsResponse(data, totalCount, request.Page, request.PageSize));
    }
}
