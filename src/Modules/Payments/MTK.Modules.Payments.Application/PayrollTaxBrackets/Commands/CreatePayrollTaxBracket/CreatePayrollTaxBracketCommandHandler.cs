using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.PayrollTaxBrackets;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.PayrollTaxBrackets.Commands.CreatePayrollTaxBracket;

internal sealed class CreatePayrollTaxBracketCommandHandler
    : ICommandHandler<CreatePayrollTaxBracketCommand, PayrollTaxBracketResponse>
{
    private readonly IPayrollTaxBracketRepository _payrollTaxBracketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePayrollTaxBracketCommandHandler(
        IPayrollTaxBracketRepository payrollTaxBracketRepository,
        IUnitOfWork unitOfWork)
    {
        _payrollTaxBracketRepository = payrollTaxBracketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PayrollTaxBracketResponse>> Handle(
        CreatePayrollTaxBracketCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UpperBound is not null && request.UpperBound <= request.LowerBound)
        {
            return Result.Failure<PayrollTaxBracketResponse>(PayrollTaxBracketErrors.InvalidBounds);
        }

        // Eyni pilləni əvəz edən (üst-üstə düşən) köhnə, hələ bağlanmamış sətirləri bağla.
        var superseded = await _payrollTaxBracketRepository.ListAsync(
            b => b.RateType == request.RateType
                 && b.Party == request.Party
                 && b.LowerBound == request.LowerBound
                 && b.UpperBound == request.UpperBound
                 && b.EffectiveTo == null,
            cancellationToken);

        foreach (var old in superseded)
        {
            old.Deactivate(request.EffectiveFrom);
        }

        var bracket = PayrollTaxBracket.Create(
            request.RateType,
            request.Party,
            request.LowerBound,
            request.UpperBound,
            request.Rate,
            request.EffectiveFrom,
            request.Description);

        _payrollTaxBracketRepository.Add(bracket);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(PayrollTaxBracketResponse.FromDomain(bracket));
    }
}
