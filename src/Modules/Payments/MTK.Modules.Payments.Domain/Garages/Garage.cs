using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.PropertyOwnerships;

namespace MTK.Modules.Payments.Domain.Garages;

/// <summary>
/// Buildings-dəki Garage-ın bu moduldakı adı-snapshot-u (Id = Buildings Garage.Id).
/// Mülkiyyət/billing üçün deyil — bu, PropertyOwnership-in işidir və ona
/// toxunulmayıb; bu, sırf "bu qaraj konkret olaraq nədir" sualına modullar arası
/// DB join olmadan cavab vermək üçündür (qəbz və s.). Apartment-dən fərqli olaraq
/// bir binaya bağlı deyil — Buildings-in öz domain-ində Garage heç bir BuildingId
/// daşımır (kompleksin ümumi əmlakıdır), ona görə burada da Building naviqasiyası yoxdur.
/// Buildings-in GarageCreated/GarageOwnerChanged integration event-ləri ilə sinxronlaşır.
/// </summary>
public sealed class Garage : Entity
{
    private Garage(Guid id, string garageNumber, GarageType garageType) : base(id)
    {
        GarageNumber = garageNumber;
        GarageType = garageType;
    }

    private Garage() : base() { }

    public string GarageNumber { get; private set; } = string.Empty;
    public GarageType GarageType { get; private set; }

    public static Garage Create(Guid id, string garageNumber, GarageType garageType)
    {
        var garage = new Garage(id, garageNumber, garageType);
        garage.SetCreatedAt();
        return garage;
    }

    public void UpdateDetails(string garageNumber, GarageType garageType)
    {
        GarageNumber = garageNumber;
        GarageType = garageType;
        SetUpdatedAt();
    }
}
