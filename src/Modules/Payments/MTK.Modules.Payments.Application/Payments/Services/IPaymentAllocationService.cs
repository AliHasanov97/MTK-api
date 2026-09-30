using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.Payments.Services;

/// <summary>
/// Ödənişlərin borclara paylanması. Heç bir metod <b>yadda saxlamır</b> — çağıran
/// tərəf vahid iş çərçivəsinə sahibdir ki, borc/ödəniş dəyişikliyi ilə onların
/// paylanması bir tranzaksiyada bitsin (əvvəllər servis özü SaveChanges çağırırdı və
/// yarımçıq vəziyyət qalırdı).
/// </summary>
public interface IPaymentAllocationService
{
    /// <summary>
    /// Ödənişi sahibin ən köhnə açıq borclarına FIFO ilə paylayır. <paramref name="propertyId"/>
    /// verilərsə yalnız həmin əmlakın borcları uyğundur.
    /// </summary>
    Task<Result> AllocatePaymentAsync(Guid paymentId, Guid ownerId, decimal amount, Guid? propertyId, CancellationToken cancellationToken);

    /// <summary>
    /// Yeni yaranmış borcları sahibin əvvəlki ödənişlərindən qalan avansla bağlayır
    /// (ən köhnə ödəniş əvvəl). Bütün borclar bir dəfə emal olunur: sahib üzrə avans
    /// bir dəfə oxunur və yaddaşda bölünür, ona görə borc sayı nə qədər olsa da əlavə
    /// sorğu/yazma yaranmır.
    /// </summary>
    Task<Result> ApplyAdvanceToChargesAsync(IReadOnlyCollection<Charge> charges, CancellationToken cancellationToken);

    /// <summary>
    /// Sahibin qalan avansını bütün açıq borclara yenidən paylayır. Ödəniş ləğv
    /// edildikdə çağırılır: ləğv digər ödənişlərin pulunu azad edir və o pul növbəti
    /// borc yaranana kimi boş qalmamalıdır.
    /// </summary>
    Task<Result> ReallocateAdvanceAsync(Guid ownerId, CancellationToken cancellationToken);
}
