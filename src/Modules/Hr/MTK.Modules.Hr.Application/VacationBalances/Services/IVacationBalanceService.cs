using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.VacationBalances;

namespace MTK.Modules.Hr.Application.VacationBalances.Services;

/// <summary>
/// Verilmiş tarixi əhatə edən iş ili üçün işçinin VacationBalance sətrini qaytarır —
/// yoxdursa, əvvəlki ildən qalan (limitsiz keçən) günü və cari staja/ailə vəziyyətinə
/// görə əlavə günləri hesablayıb yaradır ("lazy" illik rollover, cədvəlləşdirilmiş
/// job əvəzinə — hər iş ilinin başlanğıcı işçiyə görə fərqlidir).
/// </summary>
public interface IVacationBalanceService
{
    Task<Result<VacationBalance>> EnsureCurrentAsync(Guid employeeId, DateTimeOffset? asOfDate = null, CancellationToken cancellationToken = default);
}
