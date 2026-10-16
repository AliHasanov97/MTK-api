using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payroll.Commands.UpdatePayrollDeduction;

internal sealed class UpdatePayrollDeductionCommandHandler : ICommandHandler<UpdatePayrollDeductionCommand>
{
    private readonly IPayrollDeductionRepository _payrollDeductionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePayrollDeductionCommandHandler(
        IPayrollDeductionRepository payrollDeductionRepository,
        IUnitOfWork unitOfWork)
    {
        _payrollDeductionRepository = payrollDeductionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdatePayrollDeductionCommand request,
        CancellationToken cancellationToken)
    {
        var deduction = await _payrollDeductionRepository.GetByIdDefaultAsync(
            request.SalaryDeductionId,
            cancellationToken);

        if (deduction is null)
        {
            // Tutma yoxdur - ignore (idempotent)
            return Result.Success();
        }

        deduction.UpdateRemainingDebt(request.RemainingDebt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
