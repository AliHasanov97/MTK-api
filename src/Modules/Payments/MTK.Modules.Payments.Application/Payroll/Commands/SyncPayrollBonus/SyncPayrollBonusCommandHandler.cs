using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Payroll.Services;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Commands.SyncPayrollBonus;

internal sealed class SyncPayrollBonusCommandHandler : ICommandHandler<SyncPayrollBonusCommand>
{
    private readonly IPayrollBonusRepository _payrollBonusRepository;
    private readonly IPayrollRunRepository _payrollRunRepository;
    private readonly IPayrollRunGenerationService _payrollRunGenerationService;
    private readonly IUnitOfWork _unitOfWork;

    public SyncPayrollBonusCommandHandler(
        IPayrollBonusRepository payrollBonusRepository,
        IPayrollRunRepository payrollRunRepository,
        IPayrollRunGenerationService payrollRunGenerationService,
        IUnitOfWork unitOfWork)
    {
        _payrollBonusRepository = payrollBonusRepository;
        _payrollRunRepository = payrollRunRepository;
        _payrollRunGenerationService = payrollRunGenerationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SyncPayrollBonusCommand request, CancellationToken cancellationToken)
    {
        // HR-də bonus əmrləri dəyişdirilmir — idempotentlik üçün yalnız mövcud olmadıqda yaradılır.
        var existing = await _payrollBonusRepository.GetByIdDefaultAsync(request.BonusOrderId, cancellationToken);
        if (existing is not null)
        {
            return Result.Success();
        }

        // Ödənişsiz hesablama yenidən qurula bilər (aşağıda RecalculateAsync), ödənişli isə yox.
        var payrollRun = await _payrollRunRepository.GetByPeriodWithPayslipsAsync(
            request.SalaryYear,
            request.SalaryMonth,
            cancellationToken);

        if (payrollRun is not null && payrollRun.HasPayments)
        {
            return Result.Failure(PayrollErrors.PeriodAlreadyPaid(request.SalaryYear, request.SalaryMonth));
        }

        var bonus = PayrollBonus.Create(
            request.BonusOrderId,
            request.EmployeeId,
            request.BonusAmount,
            request.SalaryMonth,
            request.SalaryYear);

        _payrollBonusRepository.Add(bonus);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
