using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.SearchEducationLeaveApplications;

internal sealed class SearchEducationLeaveApplicationsQueryHandler
    : IQueryHandler<SearchEducationLeaveApplicationsQuery, SearchEducationLeaveApplicationsResponse>
{
    private readonly IEducationLeaveApplicationRepository _repository;
    private readonly IMapper _mapper;

    public SearchEducationLeaveApplicationsQueryHandler(
        IEducationLeaveApplicationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchEducationLeaveApplicationsResponse>> Handle(
        SearchEducationLeaveApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var applications = await _repository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _repository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var data = _mapper.Map<List<SearchEducationLeaveApplicationsResponseItem>>(applications);
        return Result.Success(new SearchEducationLeaveApplicationsResponse(data, totalCount, request.Page, request.PageSize));
    }
}
