using MTK.Modules.Payments.Domain.OwnerBalances;

namespace MTK.Modules.Payments.Application.OwnerBalances.Services;

/// <summary>
/// Sahib balansı proyeksiyasını (OwnerBalance cədvəli) idarə edir.
/// </summary>
public interface IOwnerBalanceService
{
    /// <summary>
    /// Balansı <b>aqreqatdan</b> (borcların cəmi + tamamlanmış ödənişlərin cəmi) mütləq
    /// yenidən hesablayır. Artırmalı (increment) yanaşma əvəzinə bu seçildi: artırma
    /// bir dənə buraxılmış/additive səhv çağırışda balansı həmişəlik səhv qoyurdu.
    ///
    /// Yadda saxlamır — çağıran tərəf vahid iş çərçivəsinə (unit of work) sahibdir,
    /// yəni borc/ödəniş dəyişiklikləri ilə balans yenilənməsi bir tranzaksiyada qalır.
    ///
    /// Hər sahib üçün yenilənmiş <see cref="OwnerBalance"/> entity-sini qaytarır (sahib
    /// tapılmasa/boş id-lərdə iştirak etmirsə, lüğətdə yoxdur) — çağıran "nə qədər
    /// borcu var" kimi mənalı sualları birbaşa entity-nin özündən (məs.
    /// <see cref="OwnerBalance.CurrentDebt"/>) soruşsun, formulanı özü təkrar yazmasın.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, OwnerBalance>> RecalculateAsync(
        IReadOnlyCollection<Guid> ownerIds, CancellationToken cancellationToken = default);
}
