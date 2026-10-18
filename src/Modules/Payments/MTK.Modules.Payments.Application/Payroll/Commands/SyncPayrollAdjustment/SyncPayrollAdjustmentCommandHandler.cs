using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Payroll.Services;
using MTK.Modules.Payments.Application.Services.AverageWage;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Commands.SyncPayrollAdjustment;

internal sealed class SyncPayrollAdjustmentCommandHandler : ICommandHandler<SyncPayrollAdjustmentCommand>
{
    private readonly IPayrollAdjustmentRepository _payrollAdjustmentRepository;
    private readonly IPayrollRunRepository _payrollRunRepository;
    private readonly IPayrollRunGenerationService _payrollRunGenerationService;
    private readonly IAverageWageCalculator _averageWageCalculator;
    private readonly IUnitOfWork _unitOfWork;

    public SyncPayrollAdjustmentCommandHandler(
        IPayrollAdjustmentRepository payrollAdjustmentRepository,
        IPayrollRunRepository payrollRunRepository,
        IPayrollRunGenerationService payrollRunGenerationService,
        IAverageWageCalculator averageWageCalculator,
        IUnitOfWork unitOfWork)
    {
        _payrollAdjustmentRepository = payrollAdjustmentRepository;
        _payrollRunRepository = payrollRunRepository;
        _payrollRunGenerationService = payrollRunGenerationService;
        _averageWageCalculator = averageWageCalculator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SyncPayrollAdjustmentCommand request, CancellationToken cancellationToken)
    {
        // HR əmrləri dəyişdirilə bilmir — idempotentlik üçün yalnız mövcud olmadıqda yaradılır.
        var existing = await _payrollAdjustmentRepository.GetByIdDefaultAsync(request.OrderId, cancellationToken);
        if (existing is not null)
        {
            return Result.Success();
        }

        // Həmin ay üçün hesablama artıq ödənilibsə, geriyə doğru düzəliş tətbiq oluna bilməz:
        // rəqəmlər dəyişsə, artıq yazılmış xərc qeydi (Transaction) ilə ziddiyyət yaranar.
        var payrollRun = await _payrollRunRepository.GetByPeriodWithPayslipsAsync(
            request.SalaryYear,
            request.SalaryMonth,
            cancellationToken);

        if (payrollRun is not null && payrollRun.HasPayments)
        {
            return Result.Failure(PayrollErrors.PeriodAlreadyPaid(request.SalaryYear, request.SalaryMonth));
        }

        decimal? fixedAmount = request.FixedAmount;

        // Əgər FixedAmount təmin edilməyibsə VƏ kompensasiyadırsa, orta əmək haqqı hesabla
        if (fixedAmount is null && request.Reason == PayrollAdjustmentReason.VacationCompensation)
        {
            var avgWageResult = await _averageWageCalculator.CalculateAsync(
                request.EmployeeId,
                new DateTime(request.SalaryYear, request.SalaryMonth, 1),
                cancellationToken);

            // Kompensasiya günlərini hesabla: Hours / Günlük standart saat (8) = günlər
            const decimal defaultDailyHours = 8m;
            var compensatedDays = request.Hours / defaultDailyHours;
            fixedAmount = avgWageResult.AverageDailyWage * compensatedDays;
        }

        var adjustment = PayrollAdjustment.Create(
            request.OrderId,
            request.EmployeeId,
            request.Reason,
            request.Hours,
            request.NormHours,
            request.SalaryMonth,
            request.SalaryYear,
            fixedAmount);

        _payrollAdjustmentRepository.Add(adjustment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Hesablama artıq varsa (ödənişsizdir), düzəliş dərhal tətbiq olunsun — əks halda
        // "yaradıldı, amma heç bir run-a düşmədi" vəziyyəti yaranır.
        if (payrollRun is not null)
        {
            var recalculated = await _payrollRunGenerationService.RecalculateAsync(payrollRun.Id, cancellationToken);
            if (recalculated.IsFailure)
            {
                return Result.Failure(recalculated.Error);
            }
        }

        return Result.Success();
    }
}
