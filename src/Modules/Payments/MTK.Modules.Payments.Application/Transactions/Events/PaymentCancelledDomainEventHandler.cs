using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Payments.Events;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Application.Transactions.Events;

/// <summary>
/// Cancelling a payment means the money goes back, so the ledger gets the opposing
/// entry instead of the original one being removed — history stays traceable.
/// </summary>
internal sealed class PaymentCancelledDomainEventHandler : DomainEventHandler<PaymentCancelledDomainEvent>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentCancelledDomainEventHandler(
        IPaymentRepository paymentRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task Handle(
        PaymentCancelledDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdDefaultAsync(domainEvent.PaymentId, cancellationToken);

        // Nothing readable to reverse — leave the ledger as it is.
        if (payment is null)
        {
            return;
        }

        // Dated at the moment of cancellation, which is when the money went back.
        _transactionRepository.Add(Transaction.ForPaymentReversal(payment, domainEvent.OccurredOnUtc));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
