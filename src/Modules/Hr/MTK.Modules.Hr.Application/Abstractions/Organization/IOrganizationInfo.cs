namespace MTK.Modules.Hr.Application.Abstractions.Organization;

/// <summary>
/// MTK-nın sənədlərdə (əmr, ərizə, tabel) istifadə olunan rekvizitləri. Appsettings-dəki "Hr:Organization" bölməsindən oxunur.
/// </summary>
public interface IOrganizationInfo
{
    string Name { get; }

    /// <summary>MTK sədri (sənədlərdə imza sahəsi); boşdursa xətt qoyulur.</summary>
    string? Director { get; }

    /// <summary>MTK-nın ünvanı (əmr və ərizələrin başlığında).</summary>
    string Address { get; }
}
