using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Domain.VacationBalances;

/// <summary>
/// Əmək Məcəlləsinə görə əlavə məzuniyyət günlərinin hesablanması — saf, DB-dən
/// asılı olmayan domain xidməti. <see cref="AdditionalVacationDaysBreakdown"/>-un
/// şərhlərində yazılmış qaydaların faktiki tətbiqidir (əvvəllər bu sinif heç yerdə
/// doldurulmurdu).
/// </summary>
public static class VacationEntitlementCalculator
{
    public static AdditionalVacationDaysBreakdown Calculate(Employee employee, DateTimeOffset asOfDate)
    {
        var experienceYears = employee.CalculateTotalWorkExperience(asOfDate).Years;

        var experienceBonusDays = experienceYears switch
        {
            >= 15 => 6,
            >= 10 => 4,
            >= 5 => 2,
            _ => 0
        };

        var karabakhWorkerDays = employee.IsKarabakhWorker ? 5 : 0;
        var disabledChildDays = employee.HasDisabledChild ? 5 : 0;

        // Maddə 116: qadınlar və tək valideyin kişilər üçün, 14 yaşınadək 2+ uşaq.
        var eligibleForChildrenBonus = employee.Gender == Gender.Female || employee.IsSingleParent;
        var childrenUnder14Count = employee.ChildrenUnder14Count ?? 0;
        var childrenUnder14Days = !eligibleForChildrenBonus
            ? 0
            : childrenUnder14Count switch
            {
                >= 3 => 5,
                2 => 2,
                _ => 0
            };

        return new AdditionalVacationDaysBreakdown
        {
            ExperienceBonusDays = experienceBonusDays,
            KarabakhWorkerDays = karabakhWorkerDays,
            ChildrenUnder14Days = childrenUnder14Days,
            DisabledChildDays = disabledChildDays,
            TotalAdditionalDays = experienceBonusDays + karabakhWorkerDays + childrenUnder14Days + disabledChildDays
        };
    }
}
