using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmployeeEducationHistories;

namespace MTK.Modules.Hr.Application.EmployeeEducationHistories.GetEmployeeEducationHistories;

internal sealed class GetEmployeeEducationHistoriesQueryHandler
    : IQueryHandler<GetEmployeeEducationHistoriesQuery, GetEmployeeEducationHistoriesResponse>
{
    private readonly IEmployeeEducationHistoryRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public GetEmployeeEducationHistoriesQueryHandler(
        IEmployeeEducationHistoryRepository repository,
        IEmployeeRepository employeeRepository,
        IMapper mapper)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetEmployeeEducationHistoriesResponse>> Handle(
        GetEmployeeEducationHistoriesQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<GetEmployeeEducationHistoriesResponse>(EmployeeEducationHistoryErrors.EmployeeNotFound);

        var histories = await _repository.GetByEmployeeIdAsync(request.EmployeeId, cancellationToken);

        return Result.Success(new GetEmployeeEducationHistoriesResponse
        {
            data = _mapper.Map<List<EmployeeEducationHistoryItem>>(histories)
        });
    }
}
