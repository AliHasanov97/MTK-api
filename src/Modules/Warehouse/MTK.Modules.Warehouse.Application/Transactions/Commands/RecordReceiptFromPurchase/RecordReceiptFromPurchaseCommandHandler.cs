using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Application.Abstractions.Data;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application.Transactions.Commands.RecordReceiptFromPurchase;

internal sealed class RecordReceiptFromPurchaseCommandHandler : ICommandHandler<RecordReceiptFromPurchaseCommand>
{
    /// <summary>Alış qəbulundan yaranan əməliyyatların istinad tipi.</summary>
    public const string PurchaseReferenceType = "Purchase";

    private readonly IWarehouseTransactionRepository _transactionRepository;
    private readonly INomenclatureRepository _nomenclatureRepository;
    private readonly IWarehouseStockRepository _stockRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordReceiptFromPurchaseCommandHandler(
        IWarehouseTransactionRepository transactionRepository,
        INomenclatureRepository nomenclatureRepository,
        IWarehouseStockRepository stockRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _nomenclatureRepository = nomenclatureRepository;
        _stockRepository = stockRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RecordReceiptFromPurchaseCommand request, CancellationToken cancellationToken)
    {
        if (request.Lines.Count == 0)
        {
            return Result.Success();
        }

        // Idempotentlik: integration event təkrar çatdırıla bilər (at-least-once).
        // Alış üçün artıq əməliyyat yazılıbsa, stoku ikinci dəfə artırmırıq.
        if (await _transactionRepository.ExistsByReferenceAsync(
                PurchaseReferenceType,
                request.PurchaseId,
                cancellationToken))
        {
            return Result.Success();
        }

        // Normalize event timestamps at the persistence boundary; PostgreSQL timestamptz requires UTC offset zero.
        var receivedOnUtc = request.ReceivedOnUtc.ToUniversalTime();

        var nomenclatureIds = request.Lines.Select(l => l.NomenclatureId).Distinct().ToList();

        // Silinmiş (soft delete) nomenklaturalar repository sorğusunda görünmür —
        // onların sətirlərini ötürürük ki, alışın qalan sətirləri yenə də yazılsın.
        var nomenclatures = await _nomenclatureRepository.ListFromIdsAsync(nomenclatureIds, cancellationToken);
        var knownNomenclatureIds = nomenclatures.Select(n => n.Id).ToHashSet();

        var lines = request.Lines
            .Where(l => knownNomenclatureIds.Contains(l.NomenclatureId))
            .ToList();

        if (lines.Count == 0)
        {
            return Result.Success();
        }

        // Mövcud stokları bir dəfə oxuyuruq (WarehouseStock-un Id-si NomenclatureId-dir).
        var stocks = (await _stockRepository.ListFromIdsAsync(nomenclatureIds, cancellationToken))
            .ToDictionary(s => s.NomenclatureId);

        foreach (var line in lines)
        {
            var transaction = WarehouseTransaction.RecordReceipt(
                line.NomenclatureId,
                line.Quantity,
                line.UnitPrice,
                receivedOnUtc,
                PurchaseReferenceType,
                request.PurchaseId);

            _transactionRepository.Add(transaction);

            if (stocks.TryGetValue(line.NomenclatureId, out var stock))
            {
                stock.IncreaseStock(line.Quantity, receivedOnUtc);
            }
            else
            {
                stock = WarehouseStock.Create(line.NomenclatureId);
                stock.IncreaseStock(line.Quantity, receivedOnUtc);
                _stockRepository.Add(stock);
                stocks[line.NomenclatureId] = stock;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
