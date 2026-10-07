using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Application.Abstractions.Data;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application.Transactions.Commands.RecordReceipt;

internal sealed class RecordReceiptCommandHandler : ICommandHandler<RecordReceiptCommand, Guid>
{
    private readonly IWarehouseTransactionRepository _transactionRepository;
    private readonly INomenclatureRepository _nomenclatureRepository;
    private readonly IWarehouseStockRepository _stockRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordReceiptCommandHandler(
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

    public async Task<Result<Guid>> Handle(RecordReceiptCommand request, CancellationToken cancellationToken)
    {
        // Nomenclature mövcuddur?
        var nomenclature = await _nomenclatureRepository.GetByIdAsync(request.NomenclatureId, cancellationToken);
        if (nomenclature is null)
        {
            return Result.Failure<Guid>(WarehouseTransactionErrors.NomenclatureNotFound);
        }

        // Transaction yaradırıq
        var transaction = WarehouseTransaction.RecordReceipt(
            request.NomenclatureId,
            request.Quantity,
            request.UnitPrice,
            request.TransactionDate,
            request.ReferenceType,
            request.ReferenceId,
            request.Notes,
            request.CreatedByUserId);

        _transactionRepository.Add(transaction);

        // Stock-u artırırıq (Domain Event vasitəsilə də ola bilər, amma burada birbaşa edirik)
        var stock = await _stockRepository.GetByNomenclatureIdAsync(request.NomenclatureId, cancellationToken);

        if (stock is null)
        {
            // İlk dəfə mal gəlişi - yeni stock qeydi yaradırıq
            stock = WarehouseStock.Create(request.NomenclatureId);
            stock.IncreaseStock(request.Quantity, request.TransactionDate);
            _stockRepository.Add(stock);
        }
        else
        {
            // Mövcud stoku artırırıq
            stock.IncreaseStock(request.Quantity, request.TransactionDate);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(transaction.Id);
    }
}
