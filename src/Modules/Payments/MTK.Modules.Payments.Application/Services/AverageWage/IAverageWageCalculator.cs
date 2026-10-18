namespace MTK.Modules.Payments.Application.Services.AverageWage;

/// <summary>
/// Azərbaycan Əmək Məcəlləsinə (Maddə 145-146) uyğun orta əmək haqqı hesablaması.
///
/// Hesablama qaydası:
/// 1. Referans tarixindən əvvəlki 12 təqvim ayının maaşları götürülür
/// 2. Hər ayın brüt maaşı: BaseSalary + Bonuses + Adjustments
/// 3. Orta aylıq: 12 ayın cəmi ÷ faktiki ay sayı
/// 4. Orta günlük: Orta aylıq ÷ 30.4 (365 gün ÷ 12 ay)
/// </summary>
public interface IAverageWageCalculator
{
    /// <summary>
    /// İşçinin orta əmək haqqını hesablayır.
    /// </summary>
    /// <param name="employeeId">İşçinin ID-si</param>
    /// <param name="referenceDate">Referans tarixi (məzuniyyət başlanğıc tarixi)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Orta əmək haqqı nəticəsi</returns>
    Task<AverageWageResult> CalculateAsync(
        Guid employeeId,
        DateTime referenceDate,
        CancellationToken cancellationToken = default);
}
