using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmployeeEducationHistories;

namespace MTK.Modules.Hr.Application.EmployeeEducationHistories.SearchEmployeeEducationHistories;

internal sealed class SearchEmployeeEducationHistoriesQueryHandler
    : IQueryHandler<SearchEmployeeEducationHistoriesQuery, SearchEmployeeEducationHistoriesResponse>
{
    private readonly IEmployeeEducationHistoryRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public SearchEmployeeEducationHistoriesQueryHandler(
        IEmployeeEducationHistoryRepository repository,
        IEmployeeRepository employeeRepository,
        IMapper mapper)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchEmployeeEducationHistoriesResponse>> Handle(
        SearchEmployeeEducationHistoriesQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<SearchEmployeeEducationHistoriesResponse>(EmployeeEducationHistoryErrors.EmployeeNotFound);

        var (items, totalCount) = await _repository.SearchByEmployeeIdAsync(
            request.EmployeeId,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var data = _mapper.Map<List<SearchEmployeeEducationHistoriesResponseItem>>(items);
        return Result.Success(new SearchEmployeeEducationHistoriesResponse(data, totalCount, request.Page, request.PageSize));
    }
}
