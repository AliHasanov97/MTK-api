namespace MTK.Modules.Buildings.Domain.Appeals;

/// <summary>
/// Müraciət prioriteti.
/// İstifadəçi yaradarkən Normal təyin edilir, admin dəyişə bilər.
/// </summary>
public enum AppealPriority
{
    /// <summary>Normal prioritet (default).</summary>
    Normal = 1,

    /// <summary>Yüksək prioritet - tez həll edilməlidir.</summary>
    High = 2,

    /// <summary>Təcili - dərhal diqqət tələb edir (su axması, qaz sızması).</summary>
    Urgent = 3
}
