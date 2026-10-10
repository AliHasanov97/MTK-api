using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.LaborCodeCases;

namespace MTK.Modules.Hr.Application.LaborCodeCases.SearchLaborCodeCases;

internal sealed class SearchLaborCodeCasesQueryHandler : IQueryHandler<SearchLaborCodeCasesQuery, SearchLaborCodeCasesResponse>
{
    private readonly ILaborCodeCaseRepository _repository;
    private readonly IMapper _mapper;

    public SearchLaborCodeCasesQueryHandler(ILaborCodeCaseRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchLaborCodeCasesResponse>> Handle(SearchLaborCodeCasesQuery request, CancellationToken cancellationToken)
    {
        var cases = await _repository.SearchAsync(
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

        var data = _mapper.Map<List<SearchLaborCodeCasesResponseItem>>(cases);
        return Result.Success(new SearchLaborCodeCasesResponse(data, totalCount, request.Page, request.PageSize));
    }
}
