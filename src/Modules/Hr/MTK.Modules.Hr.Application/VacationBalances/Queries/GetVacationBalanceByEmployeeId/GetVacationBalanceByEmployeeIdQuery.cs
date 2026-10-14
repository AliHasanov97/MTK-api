using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationBalances.Queries.GetVacationBalanceByEmployeeId;

/// <summary>Verilmiş tarixdəki (default: bu gün) cari iş ili üçün işçinin məzuniyyət balansı.</summary>
public sealed record GetVacationBalanceByEmployeeIdQuery(Guid EmployeeId, DateTimeOffset? AsOfDate = null)
    : IQuery<VacationBalanceResponse>;
