using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Commands.SyncPayrollAdjustment;

internal sealed class SyncPayrollAdjustmentCommandHandler : ICommandHandler<SyncPayrollAdjustmentCommand>
{
    private readonly IPayrollAdjustmentRepository _payrollAdjustmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncPayrollAdjustmentCommandHandler(
        IPayrollAdjustmentRepository payrollAdjustmentRepository,
        IUnitOfWork unitOfWork)
    {
        _payrollAdjustmentRepository = payrollAdjustmentRepository;
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

        var adjustment = PayrollAdjustment.Create(
            request.OrderId,
            request.EmployeeId,
            request.Reason,
            request.Hours,
            request.NormHours,
            request.SalaryMonth,
            request.SalaryYear);

        _payrollAdjustmentRepository.Add(adjustment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
