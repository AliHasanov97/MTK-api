using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Commands.SyncPayrollDeduction;

internal sealed class SyncPayrollDeductionCommandHandler : ICommandHandler<SyncPayrollDeductionCommand>
{
    private readonly IPayrollDeductionRepository _payrollDeductionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncPayrollDeductionCommandHandler(IPayrollDeductionRepository payrollDeductionRepository, IUnitOfWork unitOfWork)
    {
        _payrollDeductionRepository = payrollDeductionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SyncPayrollDeductionCommand request, CancellationToken cancellationToken)
    {
        // HR-də tutma əmrləri dəyişdirilə bilmir (yalnız yaradılır/silinir) — idempotentlik
        // üçün yalnız mövcud olmadıqda yaradılır.
        var existing = await _payrollDeductionRepository.GetByIdDefaultAsync(request.SalaryDeductionId, cancellationToken);
        if (existing is not null)
        {
            return Result.Success();
        }

        var deduction = PayrollDeduction.Create(
            request.SalaryDeductionId,
            request.EmployeeId,
            request.PercentageSalary,
            request.StartDate,
            request.RemainingDebt);

        _payrollDeductionRepository.Add(deduction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
