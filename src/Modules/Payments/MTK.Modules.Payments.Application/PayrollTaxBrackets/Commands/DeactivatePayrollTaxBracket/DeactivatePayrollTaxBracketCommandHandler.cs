using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.PayrollTaxBrackets;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.PayrollTaxBrackets.Commands.DeactivatePayrollTaxBracket;

internal sealed class DeactivatePayrollTaxBracketCommandHandler : ICommandHandler<DeactivatePayrollTaxBracketCommand>
{
    private readonly IPayrollTaxBracketRepository _payrollTaxBracketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivatePayrollTaxBracketCommandHandler(
        IPayrollTaxBracketRepository payrollTaxBracketRepository,
        IUnitOfWork unitOfWork)
    {
        _payrollTaxBracketRepository = payrollTaxBracketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeactivatePayrollTaxBracketCommand request, CancellationToken cancellationToken)
    {
        var bracket = await _payrollTaxBracketRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (bracket is null)
        {
            return Result.Failure(PayrollTaxBracketErrors.NotFound);
        }

        if (bracket.EffectiveTo is not null)
        {
            return Result.Failure(PayrollTaxBracketErrors.AlreadyDeactivated);
        }

        bracket.Deactivate(request.EffectiveTo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
