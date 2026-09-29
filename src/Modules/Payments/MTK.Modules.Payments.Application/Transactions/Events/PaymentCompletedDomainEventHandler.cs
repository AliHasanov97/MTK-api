using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Abstractions.Data;
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
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentCompletedDomainEventHandler(
        IPaymentRepository paymentRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _transactionRepository = transactionRepository;
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

        _transactionRepository.Add(Transaction.ForPayment(payment));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
