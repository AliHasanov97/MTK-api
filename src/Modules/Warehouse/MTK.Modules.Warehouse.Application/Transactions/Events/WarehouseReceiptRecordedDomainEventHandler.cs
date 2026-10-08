using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions.Events;
using MTK.Modules.Warehouse.IntegrationEvents.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application.Transactions.Events;

/// <summary>
/// Mal qəbulu qeyd edildikdə digər modullara bildiriş göndərir (uçot/hesabat üçün).
/// Domain event outbox vasitəsilə işlənir.
/// </summary>
internal sealed class WarehouseReceiptRecordedDomainEventHandler
    : DomainEventHandler<WarehouseReceiptRecordedDomainEvent>
{
    private readonly IWarehouseTransactionRepository _transactionRepository;
    private readonly IEventBus _eventBus;

    public WarehouseReceiptRecordedDomainEventHandler(
        IWarehouseTransactionRepository transactionRepository,
        IEventBus eventBus)
    {
        _transactionRepository = transactionRepository;
        _eventBus = eventBus;
    }

    public override async Task Handle(
        WarehouseReceiptRecordedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // Domain event yalnız TransactionId/NomenclatureId/miqdar daşıyır — qalan detalları
        // əməliyyatdan oxuyuruq.
        var transaction = await _transactionRepository.GetByIdAsync(
            domainEvent.TransactionId,
            cancellationToken);

        if (transaction is null)
        {
            return;
        }

        var integrationEvent = new WarehouseReceiptRecordedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            transaction.Id,
            transaction.NomenclatureId,
            transaction.Quantity,
            transaction.UnitPrice,
            transaction.TransactionDate,
            transaction.ReferenceType,
            transaction.ReferenceId);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
