using System.Globalization;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.CompanyBalances.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Payments.Events;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Application.Transactions.Events;

/// <summary>
/// A completed payment is money actually received, so it posts the matching income
/// entry to the general ledger. Driven by the domain event (through the outbox) so
/// that creating a payment stays free of ledger concerns.
/// </summary>
internal sealed class PaymentCompletedDomainEventHandler : DomainEventHandler<PaymentCompletedDomainEvent>
{
    private static readonly string[] AzMonthNames =
    [
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr"
    ];

    private readonly IPaymentRepository _paymentRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IPaymentAllocationRepository _paymentAllocationRepository;
    private readonly IChargeRepository _chargeRepository;
    private readonly IPropertyOwnershipRepository _propertyOwnershipRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICompanyBalanceService _companyBalanceService;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentCompletedDomainEventHandler(
        IPaymentRepository paymentRepository,
        IVendorRepository vendorRepository,
        IPaymentAllocationRepository paymentAllocationRepository,
        IChargeRepository chargeRepository,
        IPropertyOwnershipRepository propertyOwnershipRepository,
        ITransactionRepository transactionRepository,
        ICompanyBalanceService companyBalanceService,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _vendorRepository = vendorRepository;
        _paymentAllocationRepository = paymentAllocationRepository;
        _chargeRepository = chargeRepository;
        _propertyOwnershipRepository = propertyOwnershipRepository;
        _transactionRepository = transactionRepository;
        _companyBalanceService = companyBalanceService;
        _unitOfWork = unitOfWork;
    }

    public override async Task Handle(
        PaymentCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdDefaultAsync(domainEvent.PaymentId, cancellationToken);

        // Nothing readable to post — leave the ledger as it is.
        if (payment is null)
        {
            return;
        }

        if (payment.PartyType == PartyType.Vendor)
        {
            await PostVendorLedgerLines(payment, cancellationToken);
        }
        else
        {
            await PostOwnerLedgerLine(payment, cancellationToken);
        }

        // Must be persisted before RecalculateAsync: it sums via a DB query (GroupBy/Sum
        // translated to SQL), which doesn't see an Added-but-unsaved row — same two-phase
        // save CreateChargeCommandHandler uses before OwnerBalanceService.RecalculateAsync.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _companyBalanceService.RecalculateAsync(cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// One ledger line PER CHARGE this payment settled, each with just that
    /// charge's own amount and its own clean description — a bulk/FIFO payment
    /// covering several contract charges at once used to post as a single line
    /// for the whole payment, with every charge's description mashed together
    /// ("Lift xidməti — 2026-08, Lift xidməti — 2026-09 · Nağd"); now each shows
    /// up as its own line, like a real expense ledger.
    /// </summary>
    private async Task PostVendorLedgerLines(Payment payment, CancellationToken cancellationToken)
    {
        // Vendor lives in this same module (unlike the resident/owner's name), so the
        // ledger can name who actually got paid instead of just the payment method.
        var vendor = await _vendorRepository.GetByIdAsync(payment.PartyId, cancellationToken);
        var vendorName = vendor?.Name;

        var allocations = await _paymentAllocationRepository.GetByPaymentIdAsync(payment.Id, cancellationToken);

        if (allocations.Count == 0)
        {
            // Nothing to break down by (a vendor payment always settles at least
            // one charge in practice — vendors can't overpay into an advance —
            // but a payment somehow left unallocated still needs a line).
            _transactionRepository.Add(Transaction.ForPayment(payment, vendorName));
            return;
        }

        var chargeIds = allocations.Select(a => a.ChargeId).Distinct().ToList();
        var chargesById = (await _chargeRepository.ListFromIdsAsync(chargeIds, cancellationToken))
            .ToDictionary(c => c.Id);

        foreach (var allocation in allocations)
        {
            // Every vendor charge's own Description already names what it's for — a
            // one-time service carries just the service name; a scheduled/recurring
            // one bakes its period in too ("{service} — {period}", see
            // Charge.ForServiceSchedule).
            var description = chargesById.TryGetValue(allocation.ChargeId, out var charge) ? charge.Description : null;
            _transactionRepository.Add(
                Transaction.ForPayment(payment, vendorName, description, amount: allocation.Amount));
        }
    }

    /// <summary>
    /// One ledger line for the whole payment — an owner payment can leave part of
    /// itself as an unapplied advance (no charge to attribute it to), so unlike the
    /// vendor side it isn't cleanly splittable into "one line per charge"; it keeps
    /// naming every month/property the payment touched in a single combined line.
    /// </summary>
    private async Task PostOwnerLedgerLine(Payment payment, CancellationToken cancellationToken)
    {
        string? propertyLabel = null;
        string? periodLabel = null;

        // "Which month(s) did this payment actually pay down?" — only answerable
        // from its allocations (allocation already ran, before MarkAsCompleted
        // raised this event), not from the payment amount alone.
        var allocations = await _paymentAllocationRepository.GetByPaymentIdAsync(payment.Id, cancellationToken);
        Guid? propertyId = payment.PropertyId;

        if (allocations.Count > 0)
        {
            var chargeIds = allocations.Select(a => a.ChargeId).Distinct().ToList();
            var charges = await _chargeRepository.ListFromIdsAsync(chargeIds, cancellationToken);
            propertyId ??= charges.Select(c => c.ApartmentId ?? c.GarageId).FirstOrDefault(id => id.HasValue);

            var periods = charges
                .Select(c => c.Period)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct()
                .OrderBy(p => p)
                .ToList();
            if (periods.Count > 0)
            {
                periodLabel = string.Join(", ", periods.Select(FormatPeriod));
            }
        }

        if (propertyId.HasValue)
        {
            var ownership = await _propertyOwnershipRepository.GetByPropertyIdAsync(propertyId.Value, cancellationToken);
            if (ownership is not null)
            {
                // Real unit number comes straight from the Apartment/Garage shadow navigation.
                string? number = ownership.Garage?.GarageNumber ?? ownership.Apartment?.ApartmentNumber;

                if (!string.IsNullOrWhiteSpace(number))
                {
                    var propertyKind = ownership.GarageId.HasValue ? "qaraj" : "mənzil";
                    propertyLabel = $"{number} nömrəli {propertyKind}";
                }
            }
        }

        _transactionRepository.Add(Transaction.ForPayment(payment, propertyLabel: propertyLabel, periodLabel: periodLabel));
    }

    // Charge.Period is stored as "yyyy-MM"; render it the way residents read it.
    private static string FormatPeriod(string? period)
    {
        if (string.IsNullOrWhiteSpace(period))
        {
            return string.Empty;
        }

        if (DateTime.TryParseExact(period, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            return $"{AzMonthNames[parsed.Month - 1]} {parsed.Year}";
        }

        return period;
    }
}
