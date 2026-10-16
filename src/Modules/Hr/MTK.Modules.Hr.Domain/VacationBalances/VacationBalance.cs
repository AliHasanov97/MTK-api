using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Domain.VacationBalances;

/// <summary>
/// Bir işçinin bir iş ili (StartWorkDate-in ildönümünə görə, təqvim ili DEYİL) üzrə
/// məzuniyyət balansı. Hər iş ili üçün bir sətir — <see cref="WorkYearStart"/>/
/// <see cref="WorkYearEnd"/> ilə əhatə olunan dövr.
///
/// Əvvəllər bu balans heç yerdə izlənmirdi (bax: LeaveOverlapService və
/// VacationCompensationApplication-dakı "məzuniyyət balansı izlənmədiyi üçün..."
/// şərhləri) — bu entity həmin boşluğu doldurur.
/// </summary>
public sealed class VacationBalance : Entity
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    public DateOnly WorkYearStart { get; private set; }
    public DateOnly WorkYearEnd { get; private set; }

    /// <summary>Employee.VacationDays-in bu iş ili üçün snapshot-u (sonra dəyişsə, keçmiş illər dəyişməz qalır).</summary>
    public int BaseEntitledDays { get; private set; }

    /// <summary>Maddə 115: iş stajı bonusu.</summary>
    public int ExperienceBonusDays { get; private set; }

    /// <summary>Maddə 118-1: Qarabağ işçiləri.</summary>
    public int KarabakhWorkerDays { get; private set; }

    /// <summary>Maddə 116: 14 yaşınadək uşaqlar.</summary>
    public int ChildrenUnder14Days { get; private set; }

    /// <summary>Maddə 117: əlil uşaq.</summary>
    public int DisabledChildDays { get; private set; }

    /// <summary>Bu iş ili ərzində VacationOrder ilə götürülmüş günlər.</summary>
    public int UsedDays { get; private set; }

    /// <summary>Bu iş ili ərzində kompensasiya edilmiş (pulla ödənilmiş) günlər.</summary>
    public int CompensatedDays { get; private set; }

    public int AdditionalDays => ExperienceBonusDays + KarabakhWorkerDays + ChildrenUnder14Days + DisabledChildDays;
    public int TotalEntitledDays => BaseEntitledDays + AdditionalDays;
    public int RemainingDays => TotalEntitledDays - UsedDays - CompensatedDays;

    private VacationBalance() { }

    public static VacationBalance Create(
        Guid employeeId,
        DateOnly workYearStart,
        int baseEntitledDays,
        AdditionalVacationDaysBreakdown additionalDays)
    {
        return new VacationBalance
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            WorkYearStart = workYearStart,
            WorkYearEnd = workYearStart.AddYears(1).AddDays(-1),
            BaseEntitledDays = baseEntitledDays,
            ExperienceBonusDays = additionalDays.ExperienceBonusDays,
            KarabakhWorkerDays = additionalDays.KarabakhWorkerDays,
            ChildrenUnder14Days = additionalDays.ChildrenUnder14Days,
            DisabledChildDays = additionalDays.DisabledChildDays
        };
    }

    /// <summary>
    /// İş tarixçəsi (staj) və ya ailə/sosial vəziyyət dəyişəndə (bax: EmployeeWorkHistoryChangedDomainEventHandler)
    /// bu CARİ iş ilinin əlavə gün hesablamasını yeniləyir. BaseEntitledDays, UsedDays və
    /// CompensatedDays-ə toxunmur — yalnız staj/Qarabağ/uşaq/əlil-uşaq bonusları yenidən hesablanır.
    /// </summary>
    public void RecalculateAdditionalDays(AdditionalVacationDaysBreakdown additionalDays)
    {
        ExperienceBonusDays = additionalDays.ExperienceBonusDays;
        KarabakhWorkerDays = additionalDays.KarabakhWorkerDays;
        ChildrenUnder14Days = additionalDays.ChildrenUnder14Days;
        DisabledChildDays = additionalDays.DisabledChildDays;
        SetUpdatedAt();
    }

    /// <summary>VacationOrder yaradılanda çağırılır — balansın "istifadə olunmuş" tərəfini artırır.</summary>
    public void RegisterUsedDays(int days)
    {
        if (days > RemainingDays)
        {
            throw new InvalidOperationException(
                $"VacationBalance {Id}: {days} gün tələb olunur, amma yalnız {RemainingDays} gün qalıb");
        }

        UsedDays += days;
        SetUpdatedAt();
    }

    /// <summary>CompensationOrder yaradılanda çağırılır — balansın "kompensasiya olunmuş" tərəfini artırır.</summary>
    public void RegisterCompensatedDays(int days)
    {
        if (days > RemainingDays)
        {
            throw new InvalidOperationException(
                $"VacationBalance {Id}: {days} gün tələb olunur, amma yalnız {RemainingDays} gün qalıb");
        }

        CompensatedDays += days;
        SetUpdatedAt();
    }

    /// <summary>
    /// VacationReturnOrder (məzuniyyətdən geri çağırma) yaradılanda çağırılır — işçi məzuniyyəti vaxtından
    /// əvvəl kəsdiyi üçün istifadə olunmamış günləri geri "RegisterUsedDays"-in tərsi kimi balansa qaytarır.
    /// </summary>
    public void ReturnUnusedDays(int days)
    {
        if (days <= 0)
        {
            return;
        }

        UsedDays = Math.Max(0, UsedDays - days);
        SetUpdatedAt();
    }
}
