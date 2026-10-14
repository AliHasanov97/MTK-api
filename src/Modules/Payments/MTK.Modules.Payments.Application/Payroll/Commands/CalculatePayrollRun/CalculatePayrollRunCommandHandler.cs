using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Commands.CalculatePayrollRun;

internal sealed class CalculatePayrollRunCommandHandler : ICommandHandler<CalculatePayrollRunCommand, PayrollRunResponse>
{
    private readonly IPayrollRunRepository _payrollRunRepository;
    private readonly IPayrollTaxBracketRepository _payrollTaxBracketRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayrollBonusRepository _payrollBonusRepository;
    private readonly IPayrollDeductionRepository _payrollDeductionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CalculatePayrollRunCommandHandler(
        IPayrollRunRepository payrollRunRepository,
        IPayrollTaxBracketRepository payrollTaxBracketRepository,
        IEmployeeRepository employeeRepository,
        IPayrollBonusRepository payrollBonusRepository,
        IPayrollDeductionRepository payrollDeductionRepository,
        IUnitOfWork unitOfWork)
    {
        _payrollRunRepository = payrollRunRepository;
        _payrollTaxBracketRepository = payrollTaxBracketRepository;
        _employeeRepository = employeeRepository;
        _payrollBonusRepository = payrollBonusRepository;
        _payrollDeductionRepository = payrollDeductionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PayrollRunResponse>> Handle(CalculatePayrollRunCommand request, CancellationToken cancellationToken)
    {
        var existing = await _payrollRunRepository.GetByPeriodAsync(request.Year, request.Month, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<PayrollRunResponse>(new Error(
                "PayrollRun.AlreadyExists",
                $"{request.Year}-{request.Month:D2} dövrü üçün hesablama artıq mövcuddur (PayrollRun {existing.Id})"));
        }

        var employees = await _employeeRepository.ListAsync(cancellationToken);
        if (employees.Count == 0)
        {
            return Result.Failure<PayrollRunResponse>(new Error(
                "PayrollRun.NoEmployees",
                "HR-dan sinxronlaşmış heç bir işçi tapılmadı"));
        }

        var periodStart = new DateTimeOffset(request.Year, request.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var brackets = await _payrollTaxBracketRepository.GetEffectiveBracketsAsync(periodStart, cancellationToken);
        if (brackets.Count == 0)
        {
            return Result.Failure<PayrollRunResponse>(new Error(
                "PayrollRun.NoTaxBrackets",
                $"{request.Year}-{request.Month:D2} tarixi üçün qüvvədə olan heç bir vergi/sığorta dərəcəsi tapılmadı"));
        }

        var bonuses = await _payrollBonusRepository.GetForPeriodAsync(request.Year, request.Month, cancellationToken);
        var bonusByEmployee = bonuses
            .GroupBy(b => b.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.Amount));

        var activeDeductions = await _payrollDeductionRepository.GetActiveAsync(periodStart, cancellationToken);
        var deductionPercentageByEmployee = activeDeductions
            .GroupBy(d => d.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.PercentageSalary));

        var run = PayrollRun.Create(request.Year, request.Month);

        foreach (var employee in employees)
        {
            var bonusAmount = bonusByEmployee.GetValueOrDefault(employee.Id, 0m);
            var courtDeductionPercentage = Math.Min(100m, deductionPercentageByEmployee.GetValueOrDefault(employee.Id, 0m));

            var result = PayrollCalculator.Calculate(employee.BaseSalary, bonusAmount, courtDeductionPercentage, brackets);
            run.AddPayslip(employee.Id, employee.FullName, result);
        }

        run.MarkCalculated();

        _payrollRunRepository.Add(run);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(PayrollRunResponse.FromDomain(run));
    }
}
