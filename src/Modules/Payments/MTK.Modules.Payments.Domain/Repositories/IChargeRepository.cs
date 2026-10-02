using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;

namespace MTK.Modules.Payments.Domain.Repositories;

/// <summary>
/// Sakinə aid borclar üzrə oxuma/yazma. Tədarükçü borcları eyni <see cref="Charge"/>
/// aqreqatındadır, lakin onlara <see cref="IVendorChargeRepository"/> vasitəsilə
/// müraciət olunur (tərəfə görə scope).
/// </summary>
public interface IChargeRepository : IRepository<Charge>
{
    Task<IEnumerable<Charge>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Charge>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Charge>> GetUnpaidChargesAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Charge>> GetUnpaidChargesAsync(Guid ownerId, Guid propertyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tərəfə (sakin və ya tədarükçü) aid açıq borclar, FIFO sırası ilə. Ödəniş
    /// paylanması hər iki tərəf üçün eyni mexanizmdən istifadə etsin deyə ümumi
    /// metoddur.
    /// </summary>
    Task<IEnumerable<Charge>> GetUnpaidChargesByPartyAsync(PartyType partyType, Guid partyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Əmlak üçün bu dövrdə artıq haqq varmı — SAHİBDƏN ASILI OLMAYARAQ. Bir əmlak
    /// bir dövr üçün yalnız bir dəfə haqqlandırılmalıdır: mülkiyyət dövr içində başqa
    /// sahibə keçsə belə, əvvəlki sahibin artıq yaratdığı/ödədiyi haqq hələ də sayılır
    /// — əks halda aylıq generasiya transferdən sonra yenidən işə düşəndə yeni sahib
    /// üçün təkrar haqq yaradardı (əvvəllər məhz bu baş vermişdi: ownerId-ə görə
    /// scope edilmiş yoxlama transferi "yeni dövr" kimi görürdü).
    /// </summary>
    Task<bool> ChargeExistsForPeriodAsync(Guid propertyId, string period, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bütün sakin borcları — Period-u "{year}-" ilə başlayan (illik hesabat üçün).
    /// Period-un "yyyy-MM" formatına uyğunluğu çağıran tərəfdə yoxlanılır, çünki
    /// manual borclar fərqli bir dövr teqi daşıya bilər.
    /// </summary>
    Task<IEnumerable<Charge>> GetOwnerChargesByYearAsync(int year, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hər əmlak üzrə bütün dövrlər üzrə qalıq borc (Amount - PaidAmount cəmi,
    /// ləğv edilmiş borclar istisna) — illik hesabatın "Borc" sütunu üçün. Sahibin
    /// avans/borc balansının (<see cref="Payments.Domain.OwnerBalances.OwnerBalance"/>)
    /// FIFO-həssas həqiqi mənbəyi deyil, sadəcə həmin əmlaka açılmış borcların cəmidir.
    /// </summary>
    Task<Dictionary<Guid, decimal>> GetOutstandingAmountByPropertyIdsAsync(
        IReadOnlyCollection<Guid> propertyIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sahiblər üzrə ümumi borc cəmi (silinmişlər istisna, yalnız Owner tərəfi).
    /// Balansın mütləq yenidən hesablanması üçün — sahib başına ayrı sorğu getməsin
    /// deyə toplu işləyir.
    /// </summary>
    Task<Dictionary<Guid, decimal>> GetTotalAmountByOwnerIdsAsync(
        IReadOnlyCollection<Guid> ownerIds, CancellationToken cancellationToken = default);
}
