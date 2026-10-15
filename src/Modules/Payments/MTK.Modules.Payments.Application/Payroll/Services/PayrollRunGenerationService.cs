using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Services;

internal sealed class PayrollRunGenerationService : IPayrollRunGenerationService
{
    private readonly IPayrollRunRepository _payrollRunRepository;
    private readonly IPayrollTaxBracketRepository _payrollTaxBracketRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayrollBonusRepository _payrollBonusRepository;
    private readonly IPayrollDeductionRepository _payrollDeductionRepository;
    private readonly IPayrollAdjustmentRepository _payrollAdjustmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayrollRunGenerationService(
        IPayrollRunRepository payrollRunRepository,
        IPayrollTaxBracketRepository payrollTaxBracketRepository,
        IEmployeeRepository employeeRepository,
        IPayrollBonusRepository payrollBonusRepository,
        IPayrollDeductionRepository payrollDeductionRepository,
        IPayrollAdjustmentRepository payrollAdjustmentRepository,
        IUnitOfWork unitOfWork)
    {
        _payrollRunRepository = payrollRunRepository;
        _payrollTaxBracketRepository = payrollTaxBracketRepository;
        _employeeRepository = employeeRepository;
        _payrollBonusRepository = payrollBonusRepository;
        _payrollDeductionRepository = payrollDeductionRepository;
        _payrollAdjustmentRepository = payrollAdjustmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PayrollRunResponse>> GenerateAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        var existing = await _payrollRunRepository.GetByPeriodAsync(year, month, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<PayrollRunResponse>(new Error(
                "PayrollRun.AlreadyExists",
                $"{year}-{month:D2} dövrü üçün hesablama artıq mövcuddur (PayrollRun {existing.Id})"));
        }

        // Yalnız aktiv işçilər hesablanır — işdən çıxmış (Inactive) işçilərə qəbz yaradılmır.
        var employees = (await _employeeRepository.ListAsync(cancellationToken))
            .Where(e => e.IsActive)
            .ToList();
        if (employees.Count == 0)
        {
            return Result.Failure<PayrollRunResponse>(new Error(
                "PayrollRun.NoEmployees",
                "HR-dan sinxronlaşmış heç bir işçi tapılmadı"));
        }

        var periodStart = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        var brackets = await _payrollTaxBracketRepository.GetEffectiveBracketsAsync(periodStart, cancellationToken);
        if (brackets.Count == 0)
        {
            return Result.Failure<PayrollRunResponse>(new Error(
                "PayrollRun.NoTaxBrackets",
                $"{year}-{month:D2} tarixi üçün qüvvədə olan heç bir vergi/sığorta dərəcəsi tapılmadı"));
        }

        var bonuses = await _payrollBonusRepository.GetForPeriodAsync(year, month, cancellationToken);
        var bonusByEmployee = bonuses
            .GroupBy(b => b.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.Amount));

        var activeDeductions = await _payrollDeductionRepository.GetActiveAsync(periodStart, cancellationToken);
        var deductionPercentageByEmployee = activeDeductions
            .GroupBy(d => d.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.PercentageSalary));

        // HR əmrlərindən gələn iş saatı əsaslı düzəlişlər (məzuniyyət/ödənişsiz/kompensasiya və s.).
        var adjustments = await _payrollAdjustmentRepository.GetForPeriodAsync(year, month, cancellationToken);
        var adjustmentsByEmployee = adjustments
            .GroupBy(a => a.EmployeeId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var run = PayrollRun.Create(year, month);

        foreach (var employee in employees)
        {
            var bonusAmount = bonusByEmployee.GetValueOrDefault(employee.Id, 0m);

            // Saat əsaslı düzəliş: pul = baza maaş × (saat / ayın iş saatı norması).
            decimal adjustmentAmount = 0m;
            if (adjustmentsByEmployee.TryGetValue(employee.Id, out var employeeAdjustments))
            {
                foreach (var adjustment in employeeAdjustments)
                {
                    if (adjustment.NormHours <= 0m)
                    {
                        continue;
                    }

                    adjustmentAmount += Math.Round(
                        employee.BaseSalary * (adjustment.Hours / adjustment.NormHours),
                        2,
                        MidpointRounding.AwayFromZero);
                }
            }

            var courtDeductionPercentage = Math.Min(100m, deductionPercentageByEmployee.GetValueOrDefault(employee.Id, 0m));

            var result = PayrollCalculator.Calculate(employee.BaseSalary, bonusAmount, adjustmentAmount, courtDeductionPercentage, brackets);
            run.AddPayslip(employee.Id, employee.FullName, result);
        }

        run.MarkCalculated();

        _payrollRunRepository.Add(run);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(PayrollRunResponse.FromDomain(run));
    }
}
