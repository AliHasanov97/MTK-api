using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Application.WarehouseTransactions.GetTransactionById;

public sealed record GetTransactionByIdQuery(Guid Id) : IQuery<Result<TransactionResponse>>;

public sealed record TransactionResponse(
    Guid Id,
    TransactionType TransactionType,
    Guid NomenclatureId,
    string NomenclatureName,
    decimal Quantity,
    decimal? UnitPrice,
    decimal? TotalPrice,
    DateTimeOffset TransactionDate,
    string? ReferenceType,
    Guid? ReferenceId,
    string? Notes,
    DateTimeOffset CreatedAt);
