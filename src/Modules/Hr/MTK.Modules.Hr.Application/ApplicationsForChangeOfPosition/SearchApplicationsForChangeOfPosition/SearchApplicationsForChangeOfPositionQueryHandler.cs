using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.SearchApplicationsForChangeOfPosition;

internal sealed class SearchApplicationsForChangeOfPositionQueryHandler
    : IQueryHandler<SearchApplicationsForChangeOfPositionQuery, SearchApplicationsForChangeOfPositionResponse>
{
    private readonly IApplicationForChangeOfPositionRepository _repository;
    private readonly IMapper _mapper;

    public SearchApplicationsForChangeOfPositionQueryHandler(
        IApplicationForChangeOfPositionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchApplicationsForChangeOfPositionResponse>> Handle(
        SearchApplicationsForChangeOfPositionQuery request,
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

        var data = _mapper.Map<List<SearchApplicationsForChangeOfPositionResponseItem>>(applications);

        return Result.Success(new SearchApplicationsForChangeOfPositionResponse(data, totalCount, request.Page ?? 0, request.PageSize ?? 100));
    }
}
