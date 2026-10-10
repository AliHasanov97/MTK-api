using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.VacationReturnApplications;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.SearchVacationReturnApplications;

internal sealed class SearchVacationReturnApplicationsQueryHandler
    : IQueryHandler<SearchVacationReturnApplicationsQuery, SearchVacationReturnApplicationsResponse>
{
    private readonly IVacationReturnApplicationRepository _repository;
    private readonly IMapper _mapper;

    public SearchVacationReturnApplicationsQueryHandler(
        IVacationReturnApplicationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchVacationReturnApplicationsResponse>> Handle(
        SearchVacationReturnApplicationsQuery request,
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

        var data = _mapper.Map<List<SearchVacationReturnApplicationsResponseItem>>(applications);
        return Result.Success(new SearchVacationReturnApplicationsResponse(data, totalCount, request.Page, request.PageSize));
    }
}