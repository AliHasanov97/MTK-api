namespace MTK.Modules.Identity.Domain.Users;

/// <summary>
/// MTK (Mənzil-Tikinti Kooperativi) sistemi üçün istifadəçi rolları
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Standart istifadəçi rolu
    /// </summary>
    User = 0,

    /// <summary>
    /// Ümumi idarəçi rolu
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Sistem administratoru (tam giriş hüququ)
    /// </summary>
    Admin = 2,

    /// <summary>
    /// Mənzil sahibi - bir və ya bir neçə mənzilin sahibi olan sakin
    /// </summary>
    ApartmentOwner = 3,

    /// <summary>
    /// Bina idarəçisi (Komandant) - bina əməliyyatları və maliyyəsini idarə edir
    /// </summary>
    BuildingManager = 4,

    /// <summary>
    /// Mühasib - maliyyə əməliyyatları və hesabatları idarə edir
    /// </summary>
    Accountant = 5,

    /// <summary>
    /// İşçi - təmizlikçi, mühafizəçi, texniki işçilər
    /// </summary>
    Employee = 6
}
