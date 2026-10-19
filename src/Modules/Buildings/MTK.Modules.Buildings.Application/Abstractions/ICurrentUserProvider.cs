namespace MTK.Modules.Buildings.Application.Abstractions;

/// <summary>
/// Hazırki istifadəçi haqqında məlumat əldə edir.
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>Hazırki istifadəçinin ID-si.</summary>
    Guid UserId { get; }
}
