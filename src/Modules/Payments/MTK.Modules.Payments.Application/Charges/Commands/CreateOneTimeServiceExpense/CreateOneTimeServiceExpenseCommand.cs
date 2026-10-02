using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Charges.Commands.CreateOneTimeServiceExpense;

/// <summary>
/// Müqavilənin birdəfəlik (OneTime) xidmətlərindən birinə qarşı xərc daxil edir:
/// seçilmiş xidmət üçün tədarükçü borcu yaradılır və eyni anda ona qarşı ödəniş
/// tamamlanır (admin üçün tək addımlı "bu gün ödədim" təcrübəsi), beləcə tədarükçü
/// balansı, borc tarixçəsi və ledger qeydi avtomatik, düzgün yaranır — sərbəst
/// kateqoriyalı manual tranzaksiyadan fərqli olaraq.
///
/// No client-supplied ExpenseDate — the handler stamps it with DateTimeOffset.UtcNow.
/// </summary>
public sealed record CreateOneTimeServiceExpenseCommand(
    Guid ContractId,
    Guid ContractServiceId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    string? Notes = null) : ICommand<Guid>;
