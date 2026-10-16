using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.SalaryPayments.Queries.GetSalaryCalculationPreview;

internal sealed class GetSalaryCalculationPreviewQueryHandler
    : IQueryHandler<GetSalaryCalculationPreviewQuery, SalaryCalculationPreviewResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayrollBonusRepository _payrollBonusRepository;
    private readonly IPayrollAdjustmentRepository _payrollAdjustmentRepository;

    public GetSalaryCalculationPreviewQueryHandler(
        IEmployeeRepository employeeRepository,
        IPayrollBonusRepository payrollBonusRepository,
        IPayrollAdjustmentRepository payrollAdjustmentRepository)
    {
        _employeeRepository = employeeRepository;
        _payrollBonusRepository = payrollBonusRepository;
        _payrollAdjustmentRepository = payrollAdjustmentRepository;
    }

    public async Task<Result<SalaryCalculationPreviewResponse>> Handle(
        GetSalaryCalculationPreviewQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Fetch employee
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
        {
            return Result.Failure<SalaryCalculationPreviewResponse>(SalaryPaymentErrors.EmployeeNotFound);
        }

        if (!employee.IsActive)
        {
            return Result.Failure<SalaryCalculationPreviewResponse>(SalaryPaymentErrors.EmployeeInactive);
        }

        // 2. Fetch bonuses and adjustments for the period
        var bonuses = await _payrollBonusRepository.GetForPeriodAsync(
            request.SalaryYear, request.SalaryMonth, cancellationToken);
        var employeeBonuses = bonuses.Where(b => b.EmployeeId == request.EmployeeId).ToList();

        var adjustments = await _payrollAdjustmentRepository.GetForPeriodAsync(
            request.SalaryYear, request.SalaryMonth, cancellationToken);
        var employeeAdjustments = adjustments.Where(a => a.EmployeeId == request.EmployeeId).ToList();

        // 3. Calculate totals
        var totalBonuses = employeeBonuses.Sum(b => b.Amount);
        var totalAdjustments = employeeAdjustments.Sum(a =>
            CalculateAdjustmentAmount(employee.BaseSalary, a.Hours, a.NormHours));

        var grossSalary = employee.BaseSalary + totalBonuses + totalAdjustments;

        // 4. Build preview response
        return Result.Success(new SalaryCalculationPreviewResponse(
            employee.Id,
            employee.FullName,
            request.SalaryMonth,
            request.SalaryYear,
            employee.BaseSalary,
            employeeBonuses.Select(b => new PreviewBonusItem(
                b.Id,
                "Bonus",
                b.Amount)).ToList(),
            employeeAdjustments.Select(a => new PreviewAdjustmentItem(
                a.Id,
                GetAdjustmentDescription(a.Reason),
                a.Hours,
                a.NormHours,
                CalculateAdjustmentAmount(employee.BaseSalary, a.Hours, a.NormHours))).ToList(),
            totalBonuses,
            totalAdjustments,
            grossSalary));
    }

    private static decimal CalculateAdjustmentAmount(decimal baseSalary, decimal hours, decimal normHours)
    {
        if (normHours == 0)
            return 0;

        return Math.Round(baseSalary * (hours / normHours), 2);
    }

    private static string GetAdjustmentDescription(PayrollAdjustmentReason reason)
    {
        return reason switch
        {
            PayrollAdjustmentReason.Vacation => "Məzuniyyət",
            PayrollAdjustmentReason.VacationCompensation => "Məzuniyyət kompensasiyası",
            PayrollAdjustmentReason.UnpaidLeave => "Ödənişsiz məzuniyyət",
            PayrollAdjustmentReason.EducationLeave => "Təhsil məzuniyyəti",
            PayrollAdjustmentReason.VacationReturn => "Məzuniyyətdən qayıtma",
            PayrollAdjustmentReason.UnexcusedAbsence => "Üzrsüz qalma",
            _ => "Düzəliş"
        };
    }
}
