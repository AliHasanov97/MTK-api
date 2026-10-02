using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Application.Transactions.Commands.CreateManualTransaction;

/// <summary>
/// Posts a one-off ledger entry with no vendor/contract or resident charge behind
/// it — a utility bill paid by hand, a small ad-hoc expense, rental income, etc.
/// The deliberate exception to the ledger's usual append-only-from-payments rule.
///
/// No client-supplied TransactionDate — the handler stamps it with DateTimeOffset.UtcNow.
/// </summary>
public sealed record CreateManualTransactionCommand(
    TransactionDirection Direction,
    string Category,
    decimal Amount,
    string? Description) : ICommand<Guid>;
