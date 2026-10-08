using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.CompanyBalances.Services;
using MTK.Modules.Payments.Domain.Purchases.Events;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;
using MTK.Modules.Payments.IntegrationEvents.Warehouse;

namespace MTK.Modules.Payments.Application.Purchases.Events;

/// <summary>
/// Qəbul edilmiş alış üçün maliyyə xərcini yazır və anbara stok artımı event-i göndərir.
/// </summary>
internal sealed class PurchaseReceivedDomainEventHandler
    : DomainEventHandler<PurchaseReceivedDomainEvent>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICompanyBalanceService _companyBalanceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;

    public PurchaseReceivedDomainEventHandler(
        IPurchaseRepository purchaseRepository,
        IVendorRepository vendorRepository,
        ITransactionRepository transactionRepository,
        ICompanyBalanceService companyBalanceService,
        IUnitOfWork unitOfWork,
        IEventBus eventBus)
    {
        _purchaseRepository = purchaseRepository;
        _vendorRepository = vendorRepository;
        _transactionRepository = transactionRepository;
        _companyBalanceService = companyBalanceService;
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
    }

    public override async Task Handle(
        PurchaseReceivedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var purchase = await _purchaseRepository.GetWithLinesAsync(
            domainEvent.PurchaseId,
            cancellationToken);

        if (purchase is null)
            return;

        if (!await _transactionRepository.ExistsBySourcePurchaseIdAsync(purchase.Id, cancellationToken))
        {
            var vendor = await _vendorRepository.GetByIdAsync(purchase.VendorId, cancellationToken);
            _transactionRepository.Add(Transaction.ForPurchase(
                purchase.Id,
                purchase.TotalAmount,
                purchase.ReceivedOnUtc ?? domainEvent.ReceivedOnUtc,
                vendor?.Name,
                purchase.InvoiceNumber,
                purchase.Note));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Recalculate even on event retry: the expense row may have been saved before a prior failure.
        await _companyBalanceService.RecalculateAsync(cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var lines = purchase.Lines
            .Select(line => new GoodsReceivedLine(line.NomenclatureId, line.Quantity, line.UnitPrice))
            .ToList();

        var integrationEvent = new GoodsReceivedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            purchase.Id,
            purchase.VendorId,
            domainEvent.ReceivedOnUtc,
            lines);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
