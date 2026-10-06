using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Application.WarehouseTransactions.RecordIssue;

public sealed record RecordIssueCommand(
    Guid NomenclatureId,
    decimal Quantity,
    DateTimeOffset? TransactionDate = null,
    string? ReferenceType = null,
    Guid? ReferenceId = null,
    string? Notes = null,
    Guid? CreatedByUserId = null) : ICommand<Result<Guid>>;
