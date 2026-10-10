namespace MTK.Modules.Hr.Application.Abstractions.Authentication;

/// <summary>Cari istifadəçi (yaradan şəxs qeydləri üçün). Identity modulunun IUserContext-inin HR üçün dar görünüşü.</summary>
public interface IUserContext
{
    Guid UserId { get; }
}
