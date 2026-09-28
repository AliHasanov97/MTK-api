using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.OwnerBalances;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Services;

internal sealed class PaymentAllocationService : IPaymentAllocationService
{
    private readonly IChargeRepository _chargeRepository;
    private readonly IOwnerBalanceRepository _ownerBalanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentAllocationService(
        IChargeRepository chargeRepository,
        IOwnerBalanceRepository ownerBalanceRepository,
        IUnitOfWork unitOfWork)
    {
        _chargeRepository = chargeRepository;
        _ownerBalanceRepository = ownerBalanceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> AllocatePaymentAsync(Guid paymentId, Guid ownerId, decimal amount, CancellationToken cancellationToken)
    {
        // Get unpaid/partially paid charges for owner (oldest first)
        var charges = await _chargeRepository.GetUnpaidChargesAsync(ownerId, cancellationToken);

        decimal remainingAmount = amount;

        // Allocate payment to charges (FIFO)
        foreach (var charge in charges)
        {
            if (remainingAmount <= 0) break;

            decimal chargeRemaining = charge.Amount - charge.PaidAmount;
            decimal allocateAmount = Math.Min(remainingAmount, chargeRemaining);

            charge.ApplyPayment(allocateAmount);
            PaymentAllocation.Create(paymentId, charge.Id, allocateAmount);

            remainingAmount -= allocateAmount;
        }

        // Update owner balance
        var balance = await _ownerBalanceRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        if (balance is null)
        {
            balance = OwnerBalance.Create(ownerId);
            _ownerBalanceRepository.Add(balance);
        }

        balance.AddPayment(amount);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
