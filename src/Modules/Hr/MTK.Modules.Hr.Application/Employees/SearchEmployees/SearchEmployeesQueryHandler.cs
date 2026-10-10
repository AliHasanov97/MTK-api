using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Application.Employees.SearchEmployees;

internal sealed class SearchEmployeesQueryHandler : IQueryHandler<SearchEmployeesQuery, SearchEmployeesResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public SearchEmployeesQueryHandler(
        IEmployeeRepository employeeRepository,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchEmployeesResponse>> Handle(SearchEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _employeeRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var data = _mapper.Map<List<SearchEmployeesResponseItem>>(employees);

        // Calculate age for each employee
        foreach (var item in data)
        {
            if (item.BirthDate.HasValue)
            {
                var today = DateTimeOffset.UtcNow;
                var birthDate = item.BirthDate.Value;
                var age = today.Year - birthDate.Year;
                
                // Adjust if birthday hasn't occurred this year
                if (birthDate.Date > today.AddYears(-age).Date)
                {
                    age--;
                }

                item.Age = age;
            }
        }

        return Result.Success(new SearchEmployeesResponse(data, totalCount, request.Page, request.PageSize));
    }
}
