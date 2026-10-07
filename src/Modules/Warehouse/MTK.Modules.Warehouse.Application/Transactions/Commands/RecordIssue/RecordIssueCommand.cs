using MTK.Common.Application.Messaging;

namespace MTK.Modules.Warehouse.Application.Transactions.Commands.RecordIssue;

public sealed record RecordIssueCommand(
    Guid NomenclatureId,
    decimal Quantity,
    DateTimeOffset? TransactionDate = null,
    string? ReferenceType = null,
    Guid? ReferenceId = null,
    string? Notes = null,
    Guid? CreatedByUserId = null) : ICommand<Guid>;
