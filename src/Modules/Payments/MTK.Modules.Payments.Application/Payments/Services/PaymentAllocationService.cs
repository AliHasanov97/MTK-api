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
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentAllocationRepository _paymentAllocationRepository;
    private readonly IOwnerBalanceRepository _ownerBalanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentAllocationService(
        IChargeRepository chargeRepository,
        IPaymentRepository paymentRepository,
        IPaymentAllocationRepository paymentAllocationRepository,
        IOwnerBalanceRepository ownerBalanceRepository,
        IUnitOfWork unitOfWork)
    {
        _chargeRepository = chargeRepository;
        _paymentRepository = paymentRepository;
        _paymentAllocationRepository = paymentAllocationRepository;
        _ownerBalanceRepository = ownerBalanceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> AllocatePaymentAsync(Guid paymentId, Guid ownerId, decimal amount, Guid? propertyId, CancellationToken cancellationToken)
    {
        // Get unpaid/partially paid charges for owner (oldest first), optionally scoped to one property
        var charges = propertyId.HasValue
            ? await _chargeRepository.GetUnpaidChargesAsync(ownerId, propertyId.Value, cancellationToken)
            : await _chargeRepository.GetUnpaidChargesAsync(ownerId, cancellationToken);

        decimal remainingAmount = amount;

        // Allocate payment to charges (FIFO)
        foreach (var charge in charges)
        {
            if (remainingAmount <= 0) break;

            decimal chargeRemaining = charge.Amount - charge.PaidAmount;
            decimal allocateAmount = Math.Min(remainingAmount, chargeRemaining);

            charge.ApplyPayment(allocateAmount);
            _paymentAllocationRepository.Add(PaymentAllocation.Create(paymentId, charge.Id, allocateAmount));

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

    public async Task<Result> SettleChargeFromAdvanceAsync(Guid chargeId, Guid ownerId, CancellationToken cancellationToken)
    {
        var charge = await _chargeRepository.GetByIdDefaultAsync(chargeId, cancellationToken);
        if (charge is null)
        {
            return Result.Failure(new Error("Charge.NotFound", $"Charge not found: {chargeId}"));
        }

        decimal remaining = charge.Amount - charge.PaidAmount;
        if (remaining <= 0)
        {
            return Result.Success();
        }

        // Oldest payment first — money doesn't care which property it originally
        // targeted; once it's sitting unapplied it's just owner-level advance.
        var payments = (await _paymentRepository.GetByOwnerIdAsync(ownerId, cancellationToken))
            .Where(p => p.Status == PaymentStatus.Completed)
            .OrderBy(p => p.PaymentDate)
            .ToList();

        if (payments.Count == 0)
        {
            return Result.Success();
        }

        var allocations = await _paymentAllocationRepository.GetByPaymentIdsAsync(
            payments.Select(p => p.Id).ToList(), cancellationToken);
        var allocatedByPayment = allocations
            .GroupBy(a => a.PaymentId)
            .ToDictionary(g => g.Key, g => g.Sum(a => a.Amount));

        bool settledAnything = false;

        foreach (var payment in payments)
        {
            if (remaining <= 0) break;

            decimal alreadyAllocated = allocatedByPayment.GetValueOrDefault(payment.Id);
            decimal available = payment.Amount - alreadyAllocated;
            if (available <= 0) continue;

            decimal applyAmount = Math.Min(available, remaining);
            charge.ApplyPayment(applyAmount);
            _paymentAllocationRepository.Add(PaymentAllocation.Create(payment.Id, charge.Id, applyAmount));

            remaining -= applyAmount;
            settledAnything = true;
        }

        if (settledAnything)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
