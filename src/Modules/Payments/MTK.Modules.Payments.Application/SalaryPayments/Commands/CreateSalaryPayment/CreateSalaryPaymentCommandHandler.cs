using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Employees;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.SalaryPayments;

namespace MTK.Modules.Payments.Application.SalaryPayments.Commands.CreateSalaryPayment;

internal sealed class CreateSalaryPaymentCommandHandler
    : ICommandHandler<CreateSalaryPaymentCommand, SalaryPaymentResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISalaryPaymentRepository _salaryPaymentRepository;
    private readonly IPayrollBonusRepository _payrollBonusRepository;
    private readonly IPayrollAdjustmentRepository _payrollAdjustmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSalaryPaymentCommandHandler(
        IEmployeeRepository employeeRepository,
        ISalaryPaymentRepository salaryPaymentRepository,
        IPayrollBonusRepository payrollBonusRepository,
        IPayrollAdjustmentRepository payrollAdjustmentRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _salaryPaymentRepository = salaryPaymentRepository;
        _payrollBonusRepository = payrollBonusRepository;
        _payrollAdjustmentRepository = payrollAdjustmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SalaryPaymentResponse>> Handle(
        CreateSalaryPaymentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Fetch employee
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
        {
            return Result.Failure<SalaryPaymentResponse>(SalaryPaymentErrors.EmployeeNotFound);
        }

        if (!employee.IsActive)
        {
            return Result.Failure<SalaryPaymentResponse>(SalaryPaymentErrors.EmployeeInactive);
        }

        // 2. Check if payment already exists for this period
        var existing = await _salaryPaymentRepository.GetByEmployeeAndPeriodAsync(
            request.EmployeeId, request.SalaryYear, request.SalaryMonth, cancellationToken);

        if (existing is not null)
        {
            return Result.Failure<SalaryPaymentResponse>(SalaryPaymentErrors.PaymentAlreadyExists);
        }

        // 3. Fetch bonuses and adjustments for the period
        var bonuses = await _payrollBonusRepository.GetForPeriodAsync(
            request.SalaryYear, request.SalaryMonth, cancellationToken);
        var employeeBonuses = bonuses.Where(b => b.EmployeeId == request.EmployeeId).ToList();

        var adjustments = await _payrollAdjustmentRepository.GetForPeriodAsync(
            request.SalaryYear, request.SalaryMonth, cancellationToken);
        var employeeAdjustments = adjustments.Where(a => a.EmployeeId == request.EmployeeId).ToList();

        // 4. Create SalaryPayment in Draft status
        var salaryPayment = SalaryPayment.Create(
            request.EmployeeId,
            employee.FullName,
            request.SalaryMonth,
            request.SalaryYear,
            employee.BaseSalary,
            request.PaymentDate,
            request.PaymentMethod,
            request.Notes);

        // 5. Add line for base salary
        salaryPayment.AddLine(
            SalaryPaymentLineType.BaseSalary,
            "Əsas maaş",
            employee.BaseSalary);

        // 6. Add lines for bonuses
        foreach (var bonus in employeeBonuses)
        {
            salaryPayment.AddLine(
                SalaryPaymentLineType.Bonus,
                $"Bonus",
                bonus.Amount,
                sourceId: bonus.Id);
        }

        // 7. Add lines for adjustments (calculate amount based on hours)
        foreach (var adjustment in employeeAdjustments)
        {
            var amount = adjustment.FixedAmount ?? CalculateAdjustmentAmount(employee.BaseSalary, adjustment.Hours, adjustment.NormHours);
            var description = GetAdjustmentDescription(adjustment.Reason, adjustment.Hours, adjustment.NormHours);

            salaryPayment.AddLine(
                SalaryPaymentLineType.Adjustment,
                description,
                amount,
                sourceId: adjustment.Id);
        }

        // 8. Save
        _salaryPaymentRepository.Add(salaryPayment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 9. Build response
        return Result.Success(MapToResponse(salaryPayment));
    }

    private static decimal CalculateAdjustmentAmount(decimal baseSalary, decimal hours, decimal normHours)
    {
        if (normHours == 0)
            return 0;

        return Math.Round(baseSalary * (hours / normHours), 2);
    }

    private static string GetAdjustmentDescription(PayrollAdjustmentReason reason, decimal hours, decimal normHours)
    {
        var reasonName = reason switch
        {
            PayrollAdjustmentReason.Vacation => "Məzuniyyət",
            PayrollAdjustmentReason.VacationCompensation => "Məzuniyyət kompensasiyası",
            PayrollAdjustmentReason.UnpaidLeave => "Ödənişsiz məzuniyyət",
            PayrollAdjustmentReason.EducationLeave => "Təhsil məzuniyyəti",
            PayrollAdjustmentReason.VacationReturn => "Məzuniyyətdən qayıtma",
            PayrollAdjustmentReason.UnexcusedAbsence => "Üzrsüz qalma",
            _ => "Düzəliş"
        };

        return $"{reasonName} ({Math.Abs(hours)} saat / {normHours} saat)";
    }

    private static SalaryPaymentResponse MapToResponse(SalaryPayment salaryPayment)
    {
        return new SalaryPaymentResponse(
            salaryPayment.Id,
            salaryPayment.EmployeeId,
            salaryPayment.EmployeeFullName,
            salaryPayment.SalaryMonth,
            salaryPayment.SalaryYear,
            salaryPayment.BaseSalary,
            salaryPayment.TotalBonuses,
            salaryPayment.TotalAdjustments,
            salaryPayment.GrossSalary,
            salaryPayment.PaymentMethod,
            salaryPayment.PaymentDate,
            salaryPayment.Status,
            salaryPayment.Notes,
            salaryPayment.Lines.Select(l => new SalaryPaymentLineResponse(
                l.Id,
                l.LineType,
                l.Description,
                l.Amount,
                l.SourceBonusId,
                l.SourceAdjustmentId)).ToList(),
            salaryPayment.CreatedAt,
            salaryPayment.UpdatedAt);
    }
}
