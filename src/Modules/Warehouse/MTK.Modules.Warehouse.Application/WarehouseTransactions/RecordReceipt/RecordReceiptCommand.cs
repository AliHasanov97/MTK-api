using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Application.WarehouseTransactions.RecordReceipt;

public sealed record RecordReceiptCommand(
    Guid NomenclatureId,
    decimal Quantity,
    decimal? UnitPrice = null,
    DateTimeOffset? TransactionDate = null,
    string? ReferenceType = null,
    Guid? ReferenceId = null,
    string? Notes = null,
    Guid? CreatedByUserId = null) : ICommand<Result<Guid>>;
