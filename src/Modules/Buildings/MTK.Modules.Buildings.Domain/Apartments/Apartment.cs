using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Enums;
using MTK.Modules.Buildings.Domain.Buildings;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.Garages;
using MTK.Modules.Buildings.Domain.Apartments.Events;

namespace MTK.Modules.Buildings.Domain.Apartments;

/// <summary>
/// Mənzil entity
/// </summary>
public sealed class Apartment : Entity
{
    private readonly List<Garage> _garages = new();

    private Apartment(
        Guid id,
        Guid buildingId,
        string apartmentNumber,
        int floor,
        decimal areaSquareMeters) : base(id)
    {
        BuildingId = buildingId;
        ApartmentNumber = apartmentNumber;
        Floor = floor;
        AreaSquareMeters = areaSquareMeters;
        Status = ApartmentStatus.Vacant;
    }

    // Private constructor for EF Core
    private Apartment() : base(Guid.Empty)
    {
    }

    public Guid BuildingId { get; private set; }
    public string ApartmentNumber { get; private set; } = string.Empty;
    public int Floor { get; private set; }
    public decimal AreaSquareMeters { get; private set; }
    public int RoomCount { get; private set; }
    public ApartmentStatus Status { get; private set; }

    // Current owner (nullable - can be vacant)
    public Guid? CurrentOwnerId { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    // Navigation
    public Building Building { get; private set; } = null!;
    public Owner? CurrentOwner { get; private set; }
    public IReadOnlyCollection<Garage> Garages => _garages.AsReadOnly();

    public static Apartment Create(
        Guid buildingId,
        string apartmentNumber,
        int floor,
        decimal areaSquareMeters,
        int roomCount)
    {
        var apartment = new Apartment(
            Guid.NewGuid(),
            buildingId,
            apartmentNumber,
            floor,
            areaSquareMeters)
        {
            RoomCount = roomCount,
            CreatedAt = DateTime.UtcNow
        };

        apartment.RaiseDomainEvent(new ApartmentCreatedDomainEvent(apartment.Id, buildingId, apartmentNumber));

        return apartment;
    }

    public void Update(decimal areaSquareMeters, int roomCount)
    {
        AreaSquareMeters = areaSquareMeters;
        RoomCount = roomCount;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ApartmentUpdatedDomainEvent(Id, BuildingId));
    }

    public void UpdateStatus(ApartmentStatus status)
    {
        var oldStatus = Status;
        Status = status;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ApartmentStatusChangedDomainEvent(Id, oldStatus, status));
    }

    public void AssignOwner(Guid ownerId)
    {
        var previousOwnerId = CurrentOwnerId;
        CurrentOwnerId = ownerId;
        Status = ApartmentStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ApartmentOwnerAssignedDomainEvent(Id, ownerId, previousOwnerId));
    }

    public void RemoveOwner()
    {
        var previousOwnerId = CurrentOwnerId;
        CurrentOwnerId = null;
        Status = ApartmentStatus.Vacant;
        UpdatedAt = DateTime.UtcNow;

        if (previousOwnerId.HasValue)
        {
            RaiseDomainEvent(new ApartmentOwnerRemovedDomainEvent(Id, previousOwnerId.Value));
        }
    }

    public void MarkInTransfer()
    {
        Status = ApartmentStatus.InTransfer;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ApartmentTransferStartedDomainEvent(Id, CurrentOwnerId));
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ApartmentDeletedDomainEvent(Id));
    }
}
