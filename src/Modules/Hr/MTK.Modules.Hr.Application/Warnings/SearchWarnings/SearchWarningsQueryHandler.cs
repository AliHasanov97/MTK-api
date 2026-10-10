using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Application.Warnings.SearchWarnings;

internal sealed class SearchWarningsQueryHandler : IQueryHandler<SearchWarningsQuery, SearchWarningsResponse>
{
    private readonly IWarningRepository _repository;
    private readonly IMapper _mapper;

    public SearchWarningsQueryHandler(
        IWarningRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchWarningsResponse>> Handle(SearchWarningsQuery request, CancellationToken cancellationToken)
    {
        var warnings = await _repository.SearchAsync(
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

        var data = _mapper.Map<List<SearchWarningsResponseItem>>(warnings);

        return Result.Success(new SearchWarningsResponse(data, totalCount, request.Page ?? 0, request.PageSize ?? 100));
    }
}
