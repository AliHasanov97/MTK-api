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
    /// </summary>
    Task RecalculateAsync(IReadOnlyCollection<Guid> ownerIds, CancellationToken cancellationToken = default);
}
