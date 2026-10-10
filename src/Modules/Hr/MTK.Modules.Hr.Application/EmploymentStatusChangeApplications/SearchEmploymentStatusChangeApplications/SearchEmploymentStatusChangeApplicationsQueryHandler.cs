using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.SearchEmploymentStatusChangeApplications;

internal sealed class SearchEmploymentStatusChangeApplicationsQueryHandler
    : IQueryHandler<SearchEmploymentStatusChangeApplicationsQuery, SearchEmploymentStatusChangeApplicationsResponse>
{
    private readonly IEmploymentStatusChangeApplicationRepository _repository;
    private readonly IMapper _mapper;

    public SearchEmploymentStatusChangeApplicationsQueryHandler(
        IEmploymentStatusChangeApplicationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchEmploymentStatusChangeApplicationsResponse>> Handle(
        SearchEmploymentStatusChangeApplicationsQuery request,
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

        var data = _mapper.Map<List<SearchEmploymentStatusChangeApplicationsResponseItem>>(applications);
        return Result.Success(new SearchEmploymentStatusChangeApplicationsResponse(data, totalCount, request.Page, request.PageSize));
    }
}
