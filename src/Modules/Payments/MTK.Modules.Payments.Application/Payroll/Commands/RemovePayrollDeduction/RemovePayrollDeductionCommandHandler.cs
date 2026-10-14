using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Commands.RemovePayrollDeduction;

internal sealed class RemovePayrollDeductionCommandHandler : ICommandHandler<RemovePayrollDeductionCommand>
{
    private readonly IPayrollDeductionRepository _payrollDeductionRepository;

    public RemovePayrollDeductionCommandHandler(IPayrollDeductionRepository payrollDeductionRepository)
    {
        _payrollDeductionRepository = payrollDeductionRepository;
    }

    public async Task<Result> Handle(RemovePayrollDeductionCommand request, CancellationToken cancellationToken)
    {
        var deduction = await _payrollDeductionRepository.GetByIdDefaultAsync(request.SalaryDeductionId, cancellationToken);
        if (deduction is null)
        {
            // Artıq silinib/heç vaxt sync olunmayıb — idempotent no-op.
            return Result.Success();
        }

        await _payrollDeductionRepository.DeleteAsync(deduction, null, cancellationToken);

        return Result.Success();
    }
}
