using MTK.Modules.Hr.Domain.VacationBalances;

namespace MTK.Modules.Hr.Application.VacationBalances.Queries.GetVacationBalanceByEmployeeId;

public sealed record VacationBalanceResponse(
    Guid EmployeeId,
    DateOnly WorkYearStart,
    DateOnly WorkYearEnd,
    int BaseEntitledDays,
    int ExperienceBonusDays,
    int KarabakhWorkerDays,
    int ChildrenUnder14Days,
    int DisabledChildDays,
    int AdditionalDays,
    int CarriedOverDays,
    int TotalEntitledDays,
    int UsedDays,
    int CompensatedDays,
    int RemainingDays)
{
    public static VacationBalanceResponse FromDomain(VacationBalance balance) => new(
        balance.EmployeeId,
        balance.WorkYearStart,
        balance.WorkYearEnd,
        balance.BaseEntitledDays,
        balance.ExperienceBonusDays,
        balance.KarabakhWorkerDays,
        balance.ChildrenUnder14Days,
        balance.DisabledChildDays,
        balance.AdditionalDays,
        balance.CarriedOverDays,
        balance.TotalEntitledDays,
        balance.UsedDays,
        balance.CompensatedDays,
        balance.RemainingDays);
}
