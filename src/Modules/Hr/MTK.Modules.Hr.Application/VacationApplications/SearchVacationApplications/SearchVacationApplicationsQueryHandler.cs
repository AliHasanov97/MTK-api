using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.VacationApplications;

namespace MTK.Modules.Hr.Application.VacationApplications.SearchVacationApplications;

internal sealed class SearchVacationApplicationsQueryHandler
    : IQueryHandler<SearchVacationApplicationsQuery, SearchVacationApplicationsResponse>
{
    private readonly IVacationApplicationRepository _repository;
    private readonly IMapper _mapper;

    public SearchVacationApplicationsQueryHandler(
        IVacationApplicationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchVacationApplicationsResponse>> Handle(
        SearchVacationApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page ?? 0;
        var pageSize = request.PageSize ?? 100;

        var items = await _repository.SearchAsync(
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

        var data = _mapper.Map<List<SearchVacationApplicationsResponseItem>>(items);

        return Result.Success(new SearchVacationApplicationsResponse(data, totalCount, page, pageSize));
    }
}