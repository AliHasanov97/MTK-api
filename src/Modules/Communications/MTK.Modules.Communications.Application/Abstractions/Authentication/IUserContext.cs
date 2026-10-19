namespace MTK.Modules.Communications.Application.Abstractions.Authentication;

/// <summary>
/// Cari istifadəçi (yaradan şəxs qeydləri üçün)
/// </summary>
public interface IUserContext
{
    Guid UserId { get; }
}
