using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VacationPays.Commands.PayVacationPay;

internal sealed class PayVacationPayCommandHandler : ICommandHandler<PayVacationPayCommand>
{
    private readonly IVacationPayRepository _vacationPayRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayVacationPayCommandHandler(
        IVacationPayRepository vacationPayRepository,
        IUnitOfWork unitOfWork)
    {
        _vacationPayRepository = vacationPayRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        PayVacationPayCommand request,
        CancellationToken cancellationToken)
    {
        // 1. VacationPay tap
        var vacationPay = await _vacationPayRepository.GetByIdDefaultAsync(
            request.VacationPayId, cancellationToken);

        if (vacationPay is null)
        {
            return Result.Failure(VacationPayErrors.NotFound);
        }

        // 2. Ödənilmiş kimi qeyd et (VacationPayPaidDomainEvent raise edir)
        try
        {
            vacationPay.MarkAsPaid(request.PaymentMethod, DateTimeOffset.UtcNow);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(new Error(
                "VacationPay.InvalidOperation",
                ex.Message));
        }

        // 3. Saxla (domain event handler transaction yaradacaq)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
