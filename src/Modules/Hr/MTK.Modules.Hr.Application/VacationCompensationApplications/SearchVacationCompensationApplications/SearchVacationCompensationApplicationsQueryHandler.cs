using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.SearchVacationCompensationApplications;

internal sealed class SearchVacationCompensationApplicationsQueryHandler
    : IQueryHandler<SearchVacationCompensationApplicationsQuery, SearchVacationCompensationApplicationsResponse>
{
    private readonly IVacationCompensationApplicationRepository _repository;
    private readonly IMapper _mapper;

    public SearchVacationCompensationApplicationsQueryHandler(
        IVacationCompensationApplicationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchVacationCompensationApplicationsResponse>> Handle(
        SearchVacationCompensationApplicationsQuery request,
        CancellationToken cancellationToken)
    {
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

        return Result.Success(new SearchVacationCompensationApplicationsResponse
        {
            Items = _mapper.Map<List<SearchVacationCompensationApplicationsResponseItem>>(items),
            TotalCount = totalCount
        });
    }
}
