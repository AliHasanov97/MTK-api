using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;
using MTK.Modules.Payments.Domain.VendorPayments.Events;

namespace MTK.Modules.Payments.Application.Transactions.Events;

/// <summary>
/// Tamamlanmış tədarükçü ödənişi pulun bizdən çıxması deməkdir — ledger-ə xərc
/// (Expense) qeydi yazılır. Event outbox vasitəsilə gəlir, ona görə ödənişin
/// yaradılması ledger məntiqindən azaddır.
/// </summary>
internal sealed class VendorPaymentCompletedDomainEventHandler
    : DomainEventHandler<VendorPaymentCompletedDomainEvent>
{
    private readonly IVendorPaymentRepository _vendorPaymentRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VendorPaymentCompletedDomainEventHandler(
        IVendorPaymentRepository vendorPaymentRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _vendorPaymentRepository = vendorPaymentRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task Handle(
        VendorPaymentCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var payment = await _vendorPaymentRepository.GetByIdDefaultAsync(domainEvent.VendorPaymentId, cancellationToken);

        // Nothing readable to post — leave the ledger as it is.
        if (payment is null)
        {
            return;
        }

        _transactionRepository.Add(Transaction.ForVendorPayment(payment));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
