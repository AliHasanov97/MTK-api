using MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPayById;

namespace MTK.Modules.Payments.Application.VacationPays.Queries.SearchVacationPays;

public sealed record SearchVacationPaysResponse(
    List<VacationPayResponse> VacationPays,
    int TotalCount,
    int Page,
    int PageSize);
