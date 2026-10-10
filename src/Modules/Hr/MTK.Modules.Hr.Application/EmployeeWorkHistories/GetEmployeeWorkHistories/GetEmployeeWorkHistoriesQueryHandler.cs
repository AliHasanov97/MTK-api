using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmployeeWorkHistories;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories.GetEmployeeWorkHistories;

internal sealed class GetEmployeeWorkHistoriesQueryHandler
    : IQueryHandler<GetEmployeeWorkHistoriesQuery, GetEmployeeWorkHistoriesResponse>
{
    private readonly IEmployeeWorkHistoryRepository _workHistoryRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public GetEmployeeWorkHistoriesQueryHandler(
        IEmployeeWorkHistoryRepository workHistoryRepository,
        IEmployeeRepository employeeRepository,
        IMapper mapper)
    {
        _workHistoryRepository = workHistoryRepository;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetEmployeeWorkHistoriesResponse>> Handle(
        GetEmployeeWorkHistoriesQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<GetEmployeeWorkHistoriesResponse>(EmployeeWorkHistoryErrors.EmployeeNotFound);

        var workHistories = await _workHistoryRepository.GetByEmployeeIdAsync(request.EmployeeId, cancellationToken);

        var response = new GetEmployeeWorkHistoriesResponse
        {
            data = _mapper.Map<List<EmployeeWorkHistoryItem>>(workHistories)
        };

        return Result.Success(response);
    }
}
