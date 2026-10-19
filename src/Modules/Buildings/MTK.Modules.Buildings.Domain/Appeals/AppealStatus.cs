namespace MTK.Modules.Buildings.Domain.Appeals;

/// <summary>
/// Müraciət statusu.
/// </summary>
public enum AppealStatus
{
    /// <summary>Təqdim edilib - yeni yaranıb, gözləmədədir.</summary>
    Submitted = 1,

    /// <summary>İşlənir - admin qəbul edib, həll üzərində işləyir.</summary>
    InProgress = 2,

    /// <summary>Həll edilib - admin tərəfindən həll olunub.</summary>
    Resolved = 3,

    /// <summary>Bağlanıb - istifadəçi həlli təsdiqləyib və ya müddət bitib.</summary>
    Closed = 4,

    /// <summary>Rədd edilib - qəbul olunmayıb.</summary>
    Rejected = 5
}
