using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Employees;


namespace MTK.Modules.Hr.Application.Employees.UpdateEmployee;

internal sealed class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand, UpdateEmployeeResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UpdateEmployeeResponse>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdWithWorkHistoriesAsync(request.Id, cancellationToken);
            if (employee is null)
                return Result.Failure<UpdateEmployeeResponse>(EmployeeErrors.NotFound);

            employee.Update(
                request.Name,
                request.Surname,
                request.FathersName,
                request.Nationality,
                request.BirthDate,
                request.FinCode,
                request.IdCardNumber,
                request.SocialSecurityNumber,
                request.ContractNumber,
                request.SalaryBankName,
                request.EmployeeBankAccountNumber,
                request.MaritalStatus,
                request.NumberOfChildren,
                request.ChildrenUnder14Count,
                request.MilitaryService,
                request.Veteran,
                request.Disability,
                request.IsKarabakhWorker,
                request.IsSingleParent,
                request.HasDisabledChild,
                request.Education,
                request.PhoneNumber,
                request.HomePhoneNumber,
                request.Email,
                request.RegisteredAddress,
                request.CurrentAddress,
                request.WorkingDays,
                request.VacationDays,
                request.IsActive,
                request.EmploymentType,
                request.Gender);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<UpdateEmployeeResponse>(employee);

            // Real-time hesablanan iş təcrübəsi
            response.TotalWorkExperience = employee.CalculateTotalWorkExperience();
            response.OrganizationWorkExperience = employee.CalculateOrganizationWorkExperience();

            return Result.Success(response);
        }
        catch (NullReferenceException)
        {
            return Result.Failure<UpdateEmployeeResponse>(EmployeeErrors.NotFound);
        }
    }
}
