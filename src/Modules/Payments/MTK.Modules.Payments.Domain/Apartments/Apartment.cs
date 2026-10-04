using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Buildings;

namespace MTK.Modules.Payments.Domain.Apartments;

/// <summary>
/// Buildings-dəki Apartment-in bu moduldakı adı-snapshot-u (Id = Buildings
/// Apartment.Id). Mülkiyyət/billing üçün deyil — bu, PropertyOwnership-in işidir və
/// ona toxunulmayıb; bu, sırf "bu mənzil konkret olaraq nədir, hansı binadadır"
/// sualına modullar arası DB join olmadan cavab vermək üçündür (qəbz və s.).
/// Buildings-in ApartmentCreated/ApartmentOwnerChanged integration event-ləri ilə
/// sinxronlaşır (hər ikisi bina adı/ünvanını da daşıyır).
/// </summary>
public sealed class Apartment : Entity
{
    private Apartment(Guid id, Guid buildingId, string apartmentNumber, decimal areaSquareMeters) : base(id)
    {
        BuildingId = buildingId;
        ApartmentNumber = apartmentNumber;
        AreaSquareMeters = areaSquareMeters;
    }

    private Apartment() : base() { }

    public Guid BuildingId { get; private set; }
    public Building Building { get; private set; } = null!;
    public string ApartmentNumber { get; private set; } = string.Empty;
    public decimal AreaSquareMeters { get; private set; }

    public static Apartment Create(Guid id, Guid buildingId, string apartmentNumber, decimal areaSquareMeters)
    {
        var apartment = new Apartment(id, buildingId, apartmentNumber, areaSquareMeters);
        apartment.SetCreatedAt();
        return apartment;
    }

    public void UpdateDetails(string apartmentNumber, decimal areaSquareMeters)
    {
        ApartmentNumber = apartmentNumber;
        AreaSquareMeters = areaSquareMeters;
        SetUpdatedAt();
    }
}
