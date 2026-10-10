using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Application.Employees.GetEmployeeById;

internal sealed class GetEmployeeByIdQueryHandler : IQueryHandler<GetEmployeeByIdQuery, GetEmployeeByIdResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetEmployeeByIdResponse>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdWithWorkHistoriesAsync(request.Id, cancellationToken);
        if (employee is null)
            return Result.Failure<GetEmployeeByIdResponse>(EmployeeErrors.NotFound);

        var response = _mapper.Map<GetEmployeeByIdResponse>(employee);

        // Real-time hesablanan iş təcrübəsi
        response.TotalWorkExperience = employee.CalculateTotalWorkExperience();
        response.OrganizationWorkExperience = employee.CalculateOrganizationWorkExperience();

        return Result.Success(response);
    }
}
