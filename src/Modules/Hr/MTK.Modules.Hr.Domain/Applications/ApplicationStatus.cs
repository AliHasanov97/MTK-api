namespace MTK.Modules.Hr.Domain.Applications;

public enum ApplicationStatus
{
    /// <summary>
    /// Təsdiq gözlənilir
    /// </summary>
    PendingApproval = 0,

    /// <summary>
    /// Əmrə çevrilmişdir
    /// </summary>
    ConvertedToOrder = 1
}