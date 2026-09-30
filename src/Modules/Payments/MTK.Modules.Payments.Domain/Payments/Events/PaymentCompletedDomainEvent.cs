using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Parties;

namespace MTK.Modules.Payments.Domain.Payments.Events;

/// <summary>
/// Ödəniş tamamlandıqda qaldırılır. <see cref="PartyType"/>-a görə handler ledger-ə
/// gəlir (sakin) və ya xərc (tədarükçü) qeydi yazır.
/// </summary>
public sealed record PaymentCompletedDomainEvent(
    Guid PaymentId,
    PartyType PartyType,
    Guid PartyId,
    decimal Amount) : DomainEvent;
