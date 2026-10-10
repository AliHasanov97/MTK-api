using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.VacationApplications.Services;

/// <summary>
/// Məzuniyyət müddətinin hesablanması (bayram və həftə sonu günləri nəzərə alınır)
/// </summary>
public interface IVacationApplicationAllocationService
{
    /// <summary>
    /// Calculate RequestedDays and EndDate from flexible input
    /// Bayram və həftə sonu günlərini nəzərə alır
    /// </summary>
    /// <param name="employeeId">İşçi ID (iş həftəsini bilmək üçün)</param>
    /// <param name="startDate">Məzuniyyətin başlanğıc tarixi</param>
    /// <param name="endDate">Məzuniyyətin son tarixi (opsional)</param>
    /// <param name="requestedDays">İstənilən məzuniyyət günü sayı (opsional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Məzuniyyət günü sayı və faktiki EndDate</returns>
    Task<Result<(int RequestedDays, DateTimeOffset EndDate)>> CalculateDaysAndEndDateAsync(
        Guid employeeId,
        DateTimeOffset startDate,
        DateTimeOffset? endDate,
        int? requestedDays,
        CancellationToken cancellationToken = default);
}
