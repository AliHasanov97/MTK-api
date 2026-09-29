using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Commands.CancelPayment;

internal sealed class CancelPaymentCommandHandler : ICommandHandler<CancelPaymentCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentAllocationRepository _paymentAllocationRepository;
    private readonly IChargeRepository _chargeRepository;
    private readonly IOwnerBalanceRepository _ownerBalanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentAllocationRepository paymentAllocationRepository,
        IChargeRepository chargeRepository,
        IOwnerBalanceRepository ownerBalanceRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _paymentAllocationRepository = paymentAllocationRepository;
        _chargeRepository = chargeRepository;
        _ownerBalanceRepository = ownerBalanceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdDefaultAsync(request.PaymentId, cancellationToken);

        if (payment is null)
        {
            return Result.Failure(new Error("Payment.NotFound", $"Ödəniş tapılmadı: {request.PaymentId}"));
        }

        if (payment.Status == PaymentStatus.Cancelled)
        {
            return Result.Failure(new Error("Payment.AlreadyCancelled", "Bu ödəniş artıq ləğv edilib"));
        }

        // Undo every charge this payment was allocated to, then remove the allocation records.
        var allocations = await _paymentAllocationRepository.GetByPaymentIdAsync(payment.Id, cancellationToken);

        foreach (var allocation in allocations)
        {
            var charge = await _chargeRepository.GetByIdDefaultAsync(allocation.ChargeId, cancellationToken);
            charge?.ReversePayment(allocation.Amount);
            allocation.Delete();
        }

        // Undo the owner-level running total.
        var ownerBalance = await _ownerBalanceRepository.GetByOwnerIdAsync(payment.OwnerId, cancellationToken);
        ownerBalance?.RemovePayment(payment.Amount);

        // Raising PaymentCancelledDomainEvent here is what posts the reversal entry:
        // PaymentCancelledDomainEventHandler writes the matching transaction.
        payment.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
