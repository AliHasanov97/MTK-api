using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPayById;

namespace MTK.Modules.Payments.Application.VacationPays.Queries.GetUpcomingVacationPays;

/// <summary>
/// Yaxınlaşan məzuniyyət haqqlarını gətir.
/// DaysAhead parametri ilə növbəti neçə gün ərzində ödənilməli olanları göstərir.
/// </summary>
public sealed record GetUpcomingVacationPaysQuery(
    int DaysAhead = 7) : IQuery<List<VacationPayResponse>>;
