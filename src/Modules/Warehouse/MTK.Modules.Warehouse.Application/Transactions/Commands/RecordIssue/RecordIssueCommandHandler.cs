using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Application.Abstractions.Data;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application.Transactions.Commands.RecordIssue;

internal sealed class RecordIssueCommandHandler : ICommandHandler<RecordIssueCommand, Guid>
{
    private readonly IWarehouseTransactionRepository _transactionRepository;
    private readonly INomenclatureRepository _nomenclatureRepository;
    private readonly IWarehouseStockRepository _stockRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordIssueCommandHandler(
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

    public async Task<Result<Guid>> Handle(RecordIssueCommand request, CancellationToken cancellationToken)
    {
        // Nomenclature mövcuddur?
        var nomenclature = await _nomenclatureRepository.GetByIdAsync(request.NomenclatureId, cancellationToken);
        if (nomenclature is null)
        {
            return Result.Failure<Guid>(WarehouseTransactionErrors.NomenclatureNotFound);
        }

        // Stock yoxlayırıq
        var stock = await _stockRepository.GetByNomenclatureIdAsync(request.NomenclatureId, cancellationToken);
        if (stock is null || !stock.HasSufficientStock(request.Quantity))
        {
            var available = stock?.QuantityOnHand ?? 0;
            return Result.Failure<Guid>(WarehouseTransactionErrors.InsufficientStock(available, request.Quantity));
        }

        // Transaction yaradırıq
        var transaction = WarehouseTransaction.RecordIssue(
            request.NomenclatureId,
            request.Quantity,
            request.TransactionDate,
            request.ReferenceType,
            request.ReferenceId,
            request.Notes,
            request.CreatedByUserId);

        _transactionRepository.Add(transaction);

        // Stock-u azaldırıq
        stock.DecreaseStock(request.Quantity, request.TransactionDate);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(transaction.Id);
    }
}
