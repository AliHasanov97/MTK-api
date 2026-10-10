using AutoMapper;
using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Jobs;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Jobs;

namespace MTK.Modules.Hr.Application.Employees.AddEmployee;

internal sealed class AddEmployeeCommandHandler : ICommandHandler<AddEmployeeCommand, AddEmployeeResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public AddEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IJobRepository jobRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext)
    {
        _employeeRepository = employeeRepository;
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<AddEmployeeResponse>> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {

            var registerNumber = await _employeeRepository.GetNextRegisterNumberAsync(cancellationToken);

            var employee = Employee.Create(
                registerNumber: registerNumber,
                name: request.Name,
                surname: request.Surname,
                fathersName: request.FathersName,
                gender: request.Gender,
                startWorkDate: request.StartWorkDate,
                jobId: request.JobId,
                createdById: _userContext.UserId,
                employmentOrderId: null,
                // Personal Info
                nationality: request.Nationality,
                birthDate: request.BirthDate,
                finCode: request.FinCode,
                idCardNumber: request.IdCardNumber,
                socialSecurityNumber: request.SocialSecurityNumber,
                contractNumber: request.ContractNumber,
                // Bank Info
                salaryBankName: request.SalaryBankName,
                employeeBankAccountNumber: request.EmployeeBankAccountNumber,
                // Family Info
                maritalStatus: request.MaritalStatus,
                numberOfChildren: request.NumberOfChildren,
                childrenUnder14Count: request.ChildrenUnder14Count,
                // Military & Status
                militaryService: request.MilitaryService,
                veteran: request.Veteran,
                disability: request.Disability,
                isKarabakhWorker: request.IsKarabakhWorker,
                isSingleParent: request.IsSingleParent,
                hasDisabledChild: request.HasDisabledChild,
                // Education
                education: request.Education,
                // Contact Info
                phoneNumber: request.PhoneNumber,
                homePhoneNumber: request.HomePhoneNumber,
                email: request.Email,
                registeredAddress: request.RegisteredAddress,
                currentAddress: request.CurrentAddress,
                // Work Info
                workingDays: request.WorkingDays,
                vacationDays: request.VacationDays,
                employmentType: request.EmploymentType);

            _employeeRepository.Add(employee);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var saved = await _employeeRepository.GetByIdDefaultAsync(employee.Id, cancellationToken);
            return Result.Success(_mapper.Map<AddEmployeeResponse>(saved));
        }
      
        catch (Exception)
        {
            return Result.Failure<AddEmployeeResponse>(
                EmployeeErrors.AddFailed);
        }
    }
}
