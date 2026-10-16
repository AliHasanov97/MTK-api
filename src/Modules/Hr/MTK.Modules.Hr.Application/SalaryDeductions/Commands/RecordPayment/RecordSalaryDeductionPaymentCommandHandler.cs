using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.SalaryDeductions;

namespace MTK.Modules.Hr.Application.SalaryDeductions.Commands.RecordPayment;

internal sealed class RecordSalaryDeductionPaymentCommandHandler : ICommandHandler<RecordSalaryDeductionPaymentCommand>
{
    private readonly ISalaryDeductionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordSalaryDeductionPaymentCommandHandler(
        ISalaryDeductionRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RecordSalaryDeductionPaymentCommand request,
        CancellationToken cancellationToken)
    {
        // İşçi üçün aktiv tutmaları tap (qalan borcu olan, FIFO)
        var activeDeductions = await _repository.GetActiveByEmployeeAsync(
            request.EmployeeId,
            cancellationToken);

        if (activeDeductions.Count == 0)
        {
            // Tutma yoxdur - ignore (idempotent)
            return Result.Success();
        }

        // FIFO: İlk tutmadan başla
        decimal remainingAmount = request.Amount;

        foreach (var deduction in activeDeductions.OrderBy(d => d.JudgementDate))
        {
            if (remainingAmount <= 0)
                break;

            var debtToApply = Math.Min(remainingAmount, deduction.RemainingDebt);

            if (debtToApply > 0)
            {
                deduction.RecordPayment(debtToApply);
                remainingAmount -= debtToApply;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
