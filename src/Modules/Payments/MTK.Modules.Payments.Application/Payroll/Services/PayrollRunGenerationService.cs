using Microsoft.Extensions.Options;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Payroll;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Services;

internal sealed class PayrollRunGenerationService : IPayrollRunGenerationService
{
    private readonly IPayrollRunRepository _payrollRunRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayrollBonusRepository _payrollBonusRepository;
    private readonly IPayrollAdjustmentRepository _payrollAdjustmentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly PayrollPeriodOptions _periodOptions;
    private readonly IUnitOfWork _unitOfWork;

    public PayrollRunGenerationService(
        IPayrollRunRepository payrollRunRepository,
        IEmployeeRepository employeeRepository,
        IPayrollBonusRepository payrollBonusRepository,
        IPayrollAdjustmentRepository payrollAdjustmentRepository,
        IDateTimeProvider dateTimeProvider,
        IOptions<PayrollPeriodOptions> periodOptions,
        IUnitOfWork unitOfWork)
    {
        _payrollRunRepository = payrollRunRepository;
        _employeeRepository = employeeRepository;
        _payrollBonusRepository = payrollBonusRepository;
        _payrollAdjustmentRepository = payrollAdjustmentRepository;
        _dateTimeProvider = dateTimeProvider;
        _periodOptions = periodOptions.Value;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PayrollRunResponse>> GenerateAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        // Əmək haqqı ay bitdikdən sonra hesablanır: yalnız keçmiş təqvim ayı üçün yeni hesablama
        // yaradıla bilər (noyabrda yalnız oktyabr). Cari/gələcək və köhnə aylar üçün bağlıdır —
        // mövcud (ödənişsiz) hesablamanı düzəltmək üçün RecalculateAsync istifadə olunur.
        var (allowedYear, allowedMonth) = _periodOptions.ResolveAllowedPeriod(_dateTimeProvider.UtcNow);
        if (year != allowedYear || month != allowedMonth)
        {
            return Result.Failure<PayrollRunResponse>(
                PayrollErrors.PeriodNotClosed(year, month, allowedYear, allowedMonth));
        }

        var existing = await _payrollRunRepository.GetByPeriodAsync(year, month, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<PayrollRunResponse>(new Error(
                "PayrollRun.AlreadyExists",
                $"{year}-{month:D2} dövrü üçün hesablama artıq mövcuddur (PayrollRun {existing.Id})"));
        }

        var payslipsResult = await BuildPayslipsAsync(year, month, cancellationToken);
        if (payslipsResult.IsFailure)
        {
            return Result.Failure<PayrollRunResponse>(payslipsResult.Error);
        }

        var run = PayrollRun.Create(year, month);
        foreach (var (employeeId, employeeFullName, result) in payslipsResult.Value)
        {
            run.AddPayslip(employeeId, employeeFullName, result);
        }

        run.MarkCalculated();

        _payrollRunRepository.Add(run);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(PayrollRunResponse.FromDomain(run));
    }

    public async Task<Result<PayrollRunResponse>> RecalculateAsync(Guid payrollRunId, CancellationToken cancellationToken = default)
    {
        var run = await _payrollRunRepository.GetWithPayslipsAsync(payrollRunId, cancellationToken);
        if (run is null)
        {
            return Result.Failure<PayrollRunResponse>(PayrollErrors.RunNotFound(payrollRunId));
        }

        // Ödəniş edilibsə rəqəmlər dəyişmir — əks halda artıq yazılmış xərc qeydi ilə ziddiyyət yaranar.
        if (run.HasPayments)
        {
            return Result.Failure<PayrollRunResponse>(PayrollErrors.HasPayments(run.Id));
        }

        var payslipsResult = await BuildPayslipsAsync(run.Year, run.Month, cancellationToken);
        if (payslipsResult.IsFailure)
        {
            return Result.Failure<PayrollRunResponse>(payslipsResult.Error);
        }

        run.Recalculate(payslipsResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(PayrollRunResponse.FromDomain(run));
    }

    /// <summary>
    /// Verilmiş dövr üçün aktiv işçilərə uyğun qəbz məlumatlarını (baza maaş + bonus +
    /// saat əsaslı HR düzəlişləri) hesablayır. Həm ilk hesablama, həm yenidən hesablama bunu
    /// eyni qaydadan istifadə edir ki, nəticə həmişə üst-üstə düşsün.
    /// </summary>
    private async Task<Result<IReadOnlyCollection<(Guid EmployeeId, string EmployeeFullName, PayrollCalculationResult Result)>>>
        BuildPayslipsAsync(int year, int month, CancellationToken cancellationToken)
    {
        // Yalnız aktiv işçilər hesablanır — işdən çıxmış (Inactive) işçilərə qəbz yaradılmır.
        var employees = (await _employeeRepository.ListAsync(cancellationToken))
            .Where(e => e.IsActive)
            .ToList();
        if (employees.Count == 0)
        {
            return Result.Failure<IReadOnlyCollection<(Guid, string, PayrollCalculationResult)>>(
                new Error("PayrollRun.NoEmployees", "HR-dan sinxronlaşmış heç bir işçi tapılmadı"));
        }

        var bonuses = await _payrollBonusRepository.GetForPeriodAsync(year, month, cancellationToken);
        var bonusByEmployee = bonuses
            .GroupBy(b => b.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.Amount));

        // HR əmrlərindən gələn iş saatı əsaslı düzəlişlər (məzuniyyət/ödənişsiz/kompensasiya və s.).
        var adjustments = await _payrollAdjustmentRepository.GetForPeriodAsync(year, month, cancellationToken);
        var adjustmentsByEmployee = adjustments
            .GroupBy(a => a.EmployeeId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var payslips = new List<(Guid EmployeeId, string EmployeeFullName, PayrollCalculationResult Result)>(employees.Count);

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

            var result = PayrollCalculator.Calculate(
                employee.BaseSalary,
                bonusAmount,
                adjustmentAmount);

            payslips.Add((employee.Id, employee.FullName, result));
        }

        return Result.Success<IReadOnlyCollection<(Guid, string, PayrollCalculationResult)>>(payslips);
    }
}
