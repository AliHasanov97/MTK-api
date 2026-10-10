using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.SalaryDeductions;

namespace MTK.Modules.Hr.Application.SalaryDeductions.AddSalaryDeduction;

internal sealed class AddSalaryDeductionCommandHandler : ICommandHandler<AddSalaryDeductionCommand, AddSalaryDeductionResponse>
{
    private readonly ISalaryDeductionRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public AddSalaryDeductionCommandHandler(
        ISalaryDeductionRepository repository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<AddSalaryDeductionResponse>> Handle(AddSalaryDeductionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);

            if (employee is null)
            {
                return Result.Failure<AddSalaryDeductionResponse>(EmployeeErrors.NotFound);
            }

            var salaryDeduction = SalaryDeduction.Create(
                request.EmployeeId,
                request.District,
                request.JudgementNo,
                request.JudgementDate,
                request.StartDate,
                request.PercentageSalary,
                request.StateFee,
                request.Creditor,
                request.Debt,
                _userContext.UserId);

            await _repository.AddAsync(salaryDeduction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new AddSalaryDeductionResponse
            {
                Id = salaryDeduction.Id,
                OrderNumber = salaryDeduction.OrderNumber
            });
        }
        catch (Exception)
        {
            return Result.Failure<AddSalaryDeductionResponse>(SalaryDeductionErrors.AddFailed);
        }
    }
}
