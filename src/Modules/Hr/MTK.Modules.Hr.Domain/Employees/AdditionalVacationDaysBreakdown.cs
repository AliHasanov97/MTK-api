namespace MTK.Modules.Hr.Domain.Employees;

/// <summary>
/// Əlavə məzuniyyət günlərinin breakdown-u
/// Hər bir növ əlavə məzuniyyət ayrı-ayrıca göstərilir
/// </summary>
public class AdditionalVacationDaysBreakdown
{
    /// <summary>
    /// İş təcrübəsi bonusu (Maddə 115: 5, 10, 15 il üçün 2, 4, 6 gün)
    /// </summary>
    public int ExperienceBonusDays { get; set; }

    /// <summary>
    /// Qarabağ və işğaldan azad olunmuş ərazilərdə çalışanlar (Maddə 118-1: 5 gün)
    /// </summary>
    public int KarabakhWorkerDays { get; set; }

    /// <summary>
    /// 14 yaşınadək 2 və ya daha çox uşağı olanlar (Maddə 116: 2 uşaq=2 gün, 3+ uşaq=5 gün)
    /// Qadınlar və tək valideyni kişilər üçün
    /// </summary>
    public int ChildrenUnder14Days { get; set; }

    /// <summary>
    /// Əlil uşaq (Maddə 117: 5 gün)
    /// </summary>
    public int DisabledChildDays { get; set; }

    /// <summary>
    /// Cəmi əlavə məzuniyyət günləri
    /// </summary>
    public int TotalAdditionalDays { get; set; }
}
