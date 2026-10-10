using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.NoticesOfChangeInWorkingConditions;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.SearchNoticesOfChangeInWorkingConditions;

internal sealed class SearchNoticesOfChangeInWorkingConditionsQueryHandler : IQueryHandler<SearchNoticesOfChangeInWorkingConditionsQuery, SearchNoticesOfChangeInWorkingConditionsResponse>
{
    private readonly INoticeOfChangeInWorkingConditionsRepository _repository;
    private readonly IMapper _mapper;

    public SearchNoticesOfChangeInWorkingConditionsQueryHandler(
        INoticeOfChangeInWorkingConditionsRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchNoticesOfChangeInWorkingConditionsResponse>> Handle(SearchNoticesOfChangeInWorkingConditionsQuery request, CancellationToken cancellationToken)
    {
        var notices = await _repository.SearchAsync(
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

        var data = _mapper.Map<List<SearchNoticesOfChangeInWorkingConditionsResponseItem>>(notices);

        return Result.Success(new SearchNoticesOfChangeInWorkingConditionsResponse(data, totalCount, request.Page ?? 0, request.PageSize ?? 100));
    }
}