using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Charges.Events;

/// <summary>
/// Borc sahibin əvvəlki ödənişindən qalan avans hesabına bağlandıqda qaldırılır.
///
/// Qeyd: bu, pul hərəkəti <b>deyil</b> — pul ödəniş anında artıq ledger-ə yazılıb
/// (<c>PaymentCompletedDomainEvent</c>). Buna görə heç bir Transaction yazılmır;
/// event yalnız iz/audit və gələcək bildirişlər üçündür.
/// </summary>
public sealed record ChargeSettledFromAdvanceDomainEvent(
    Guid ChargeId,
    Guid OwnerId,
    Guid PaymentId,
    decimal Amount) : DomainEvent;
