using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Services;

internal sealed class PaymentAllocationService(
    IChargeRepository chargeRepository,
    IPaymentRepository paymentRepository,
    IPaymentAllocationRepository paymentAllocationRepository) : IPaymentAllocationService
{
    public async Task<Result> AllocatePaymentAsync(
        Guid paymentId,
        Guid ownerId,
        decimal amount,
        Guid? propertyId,
        CancellationToken cancellationToken)
    {
        // Açıq borclar ən köhnədən sıralanmış gəlir (ChargeRepository.OrderedUnpaid).
        var charges = propertyId.HasValue
            ? await chargeRepository.GetUnpaidChargesAsync(ownerId, propertyId.Value, cancellationToken)
            : await chargeRepository.GetUnpaidChargesAsync(ownerId, cancellationToken);

        decimal remainingAmount = amount;

        foreach (var charge in charges)
        {
            if (remainingAmount <= 0)
            {
                break;
            }

            decimal allocateAmount = Math.Min(remainingAmount, charge.OutstandingAmount);
            if (allocateAmount <= 0)
            {
                continue;
            }

            charge.ApplyPayment(allocateAmount);
            paymentAllocationRepository.Add(PaymentAllocation.Create(paymentId, charge.Id, allocateAmount));

            remainingAmount -= allocateAmount;
        }

        return Result.Success();
    }

    public async Task<Result> ApplyAdvanceToChargesAsync(
        IReadOnlyCollection<Charge> charges,
        CancellationToken cancellationToken)
    {
        var openChargesByOwner = charges
            .Where(charge => charge.OutstandingAmount > 0)
            .GroupBy(charge => charge.OwnerId);

        foreach (var ownerCharges in openChargesByOwner)
        {
            // Sahib üzrə avans bir dəfə oxunur; bölgü tam yaddaşda aparılır.
            var advance = await LoadAdvanceAsync(ownerCharges.Key, cancellationToken);
            if (advance.Count == 0)
            {
                continue;
            }

            ApplyAdvance(advance, OrderByDebtAge(ownerCharges));
        }

        return Result.Success();
    }

    public async Task<Result> ReallocateAdvanceAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        var charges = (await chargeRepository.GetUnpaidChargesAsync(ownerId, cancellationToken)).ToList();
        if (charges.Count == 0)
        {
            return Result.Success();
        }

        var advance = await LoadAdvanceAsync(ownerId, cancellationToken);
        if (advance.Count == 0)
        {
            return Result.Success();
        }

        ApplyAdvance(advance, charges);

        return Result.Success();
    }

    /// <summary>
    /// Borcun yaşına görə sıra: əvvəl <see cref="Charge.IssuedOn"/>, sonra yaranma anı,
    /// əmlak tipi və Id. Son meyarlar yalnız tam eyni yaşlı borclar üçün determinizm
    /// verir (repository sıralaması ilə eyni qayda).
    /// </summary>
    private static List<Charge> OrderByDebtAge(IEnumerable<Charge> charges)
    {
        return charges
            .OrderBy(c => c.IssuedOn)
            .ThenBy(c => c.CreatedAt)
            .ThenBy(c => c.PropertyType)
            .ThenBy(c => c.Id)
            .ToList();
    }

    /// <summary>
    /// Sahibin xərclənməmiş pulu: hər tamamlanmış ödənişdən ona artıq bağlanmış
    /// paylanmalar çıxıldıqdan sonra qalan hissə, ən köhnə ödəniş əvvəl.
    ///
    /// Pul hansı əmlaka hədəfləndiyini "xatırlamır": bağlanmamış qaldıqdan sonra o,
    /// sadəcə sahib səviyyəli avansdır — avans yalnız belə yaranır, çünki əmlaka
    /// hədəflənmiş ödəniş həmin əmlakın qalıq borcundan çox ola bilmir.
    /// </summary>
    private async Task<List<(Payment Payment, decimal Available)>> LoadAdvanceAsync(
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var payments = (await paymentRepository.GetByOwnerIdAsync(ownerId, cancellationToken))
            .Where(payment => payment.Status == PaymentStatus.Completed)
            .OrderBy(payment => payment.PaymentDate)
            .ToList();

        if (payments.Count == 0)
        {
            return [];
        }

        var allocations = await paymentAllocationRepository.GetByPaymentIdsAsync(
            payments.Select(payment => payment.Id).ToList(), cancellationToken);
        var allocatedByPayment = allocations
            .GroupBy(allocation => allocation.PaymentId)
            .ToDictionary(group => group.Key, group => group.Sum(allocation => allocation.Amount));

        return payments
            .Select(payment => (Payment: payment, Available: payment.Amount - allocatedByPayment.GetValueOrDefault(payment.Id)))
            .Where(entry => entry.Available > 0)
            .ToList();
    }

    /// <summary>
    /// Avansı verilən sıra ilə borclara xərcləyir: əvvəlki borc tam ödənilmədən
    /// sonrakıya keçilmir, əvvəlki ödəniş tükənmədən sonrakı ödənişə keçilmir.
    /// </summary>
    private void ApplyAdvance(List<(Payment Payment, decimal Available)> advance, IReadOnlyList<Charge> charges)
    {
        int paymentIndex = 0;

        foreach (var charge in charges)
        {
            decimal remaining = charge.OutstandingAmount;
            if (remaining <= 0)
            {
                continue;
            }

            while (remaining > 0 && paymentIndex < advance.Count)
            {
                var (payment, available) = advance[paymentIndex];

                decimal applyAmount = Math.Min(available, remaining);
                if (applyAmount <= 0)
                {
                    paymentIndex++;
                    continue;
                }

                charge.ApplyAdvanceFrom(payment.Id, applyAmount);
                paymentAllocationRepository.Add(PaymentAllocation.Create(payment.Id, charge.Id, applyAmount));

                remaining -= applyAmount;
                available -= applyAmount;
                advance[paymentIndex] = (payment, available);

                if (available <= 0)
                {
                    paymentIndex++;
                }
            }
        }
    }
}
