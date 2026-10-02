namespace MTK.Modules.Payments.Application.CompanyBalances.Services;

/// <summary>
/// Şirkət balansı proyeksiyasını (CompanyBalance cədvəli) idarə edir.
/// </summary>
public interface ICompanyBalanceService
{
    /// <summary>
    /// Balansı Transaction jurnalından (bütün gəlir/xərc cəmləri) mütləq yenidən
    /// hesablayır — OwnerBalance ilə eyni qayda, artırma (increment) yanaşması deyil.
    ///
    /// Yadda saxlamır — çağıran tərəf vahid iş çərçivəsinə (unit of work) sahibdir,
    /// yəni tranzaksiya yazılması ilə balans yenilənməsi bir SaveChanges-də qalır.
    /// </summary>
    Task RecalculateAsync(CancellationToken cancellationToken = default);
}
