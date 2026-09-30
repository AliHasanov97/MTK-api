using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;

namespace MTK.Modules.Payments.Application.Payments.Services;

/// <summary>
/// Ödənişlərin borclara paylanması. Heç bir metod <b>yadda saxlamır</b> — çağıran
/// tərəf vahid iş çərçivəsinə sahibdir ki, borc/ödəniş dəyişikliyi ilə onların
/// paylanması bir tranzaksiyada bitsin.
///
/// Sakin və tədarükçü borcları tək <see cref="Charge"/> aqreqatında olduğu üçün
/// paylanma məntiqi hər iki tərəf üçün eynidir — tərəf <see cref="PartyType"/> ilə verilir.
/// </summary>
public interface IPaymentAllocationService
{
    /// <summary>
    /// Ödənişi tərəfin ən köhnə açıq borclarına FIFO ilə paylayır. <paramref name="propertyId"/>
    /// verilərsə (yalnız sakin) yalnız həmin əmlakın borcları uyğundur.
    /// </summary>
    Task<Result> AllocatePaymentAsync(
        Guid paymentId,
        PartyType partyType,
        Guid partyId,
        decimal amount,
        Guid? propertyId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Yeni yaranmış borcları tərəfin əvvəlki ödənişlərindən qalan avansla bağlayır
    /// (ən köhnə ödəniş əvvəl). Bütün borclar bir dəfə emal olunur: tərəf üzrə avans
    /// bir dəfə oxunur və yaddaşda bölünür.
    /// </summary>
    Task<Result> ApplyAdvanceToChargesAsync(IReadOnlyCollection<Charge> charges, CancellationToken cancellationToken);
}
