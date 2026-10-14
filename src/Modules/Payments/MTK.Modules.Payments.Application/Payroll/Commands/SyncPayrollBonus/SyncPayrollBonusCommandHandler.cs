using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Commands.SyncPayrollBonus;

internal sealed class SyncPayrollBonusCommandHandler : ICommandHandler<SyncPayrollBonusCommand>
{
    private readonly IPayrollBonusRepository _payrollBonusRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncPayrollBonusCommandHandler(IPayrollBonusRepository payrollBonusRepository, IUnitOfWork unitOfWork)
    {
        _payrollBonusRepository = payrollBonusRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SyncPayrollBonusCommand request, CancellationToken cancellationToken)
    {
        // HR-də bonus əmrləri dəyişdirilə bilmir — idempotentlik üçün yalnız mövcud olmadıqda yaradılır.
        var existing = await _payrollBonusRepository.GetByIdDefaultAsync(request.BonusOrderId, cancellationToken);
        if (existing is not null)
        {
            return Result.Success();
        }

        var bonus = PayrollBonus.Create(
            request.BonusOrderId,
            request.EmployeeId,
            request.BonusAmount,
            request.SalaryMonth,
            request.SalaryYear);

        _payrollBonusRepository.Add(bonus);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
