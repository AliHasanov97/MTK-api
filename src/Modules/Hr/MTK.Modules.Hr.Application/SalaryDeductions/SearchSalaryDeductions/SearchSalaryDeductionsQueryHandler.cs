using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.SalaryDeductions;

namespace MTK.Modules.Hr.Application.SalaryDeductions.SearchSalaryDeductions;

internal sealed class SearchSalaryDeductionsQueryHandler
    : IQueryHandler<SearchSalaryDeductionsQuery, SearchSalaryDeductionsResponse>
{
    private readonly ISalaryDeductionRepository _repository;
    private readonly IMapper _mapper;

    public SearchSalaryDeductionsQueryHandler(
        ISalaryDeductionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchSalaryDeductionsResponse>> Handle(
        SearchSalaryDeductionsQuery request,
        CancellationToken cancellationToken)
    {
        var salaryDeductions = await _repository.SearchAsync(
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

        var data = _mapper.Map<List<SearchSalaryDeductionsResponseItem>>(salaryDeductions);
        return Result.Success(new SearchSalaryDeductionsResponse(data, totalCount, request.Page ?? 1, request.PageSize ?? 10));
    }
}
