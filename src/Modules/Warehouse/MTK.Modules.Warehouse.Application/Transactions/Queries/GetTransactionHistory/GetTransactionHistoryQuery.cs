using MTK.Common.Application.Messaging;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application.Transactions.Queries.GetTransactionHistory;

public sealed record GetTransactionHistoryQuery(
    Guid? NomenclatureId = null,
    TransactionType? TransactionType = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null,
    int PageNumber = 1,
    int PageSize = 50) : IQuery<List<TransactionDto>>;

public sealed record TransactionDto(
    Guid Id,
    TransactionType TransactionType,
    Guid NomenclatureId,
    string NomenclatureName,
    decimal Quantity,
    decimal? UnitPrice,
    DateTimeOffset TransactionDate,
    string? Notes,
    string? ReferenceType,
    Guid? ReferenceId);
