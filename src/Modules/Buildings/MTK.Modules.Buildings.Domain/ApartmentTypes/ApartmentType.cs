using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.ApartmentTypes;

/// <summary>
/// Binanın konfiqurasiyasında təyin olunan mənzil tipi (otaq sayı və sahə)
/// </summary>
public sealed class ApartmentType : Entity
{
    private ApartmentType(
        Guid id,
        Guid buildingId,
        int roomCount,
        decimal areaSquareMeters) : base(id)
    {
        BuildingId = buildingId;
        RoomCount = roomCount;
        AreaSquareMeters = areaSquareMeters;
    }

    // Private constructor for EF Core
    private ApartmentType() : base()
    {
    }

    public Guid BuildingId { get; private set; }
    public int RoomCount { get; private set; }
    public decimal AreaSquareMeters { get; private set; }

    public static ApartmentType Create(
        Guid buildingId,
        int roomCount,
        decimal areaSquareMeters)
    {
        var apartmentType = new ApartmentType(Guid.NewGuid(), buildingId, roomCount, areaSquareMeters);
        apartmentType.SetCreatedAt();
        return apartmentType;
    }
}
