using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Enums;
using MTK.Modules.Buildings.Domain.Apartments;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.Garages.Events;

namespace MTK.Modules.Buildings.Domain.Garages;

/// <summary>
/// Qaraj və ya dayanacaq entity
/// </summary>
public sealed class Garage : Entity
{
    private Garage(
        Guid id,
        Guid apartmentId,
        string garageNumber,
        GarageType type) : base(id)
    {
        ApartmentId = apartmentId;
        GarageNumber = garageNumber;
        Type = type;
    }

    // Private constructor for EF Core
    private Garage() : base(Guid.Empty)
    {
    }

    public Guid ApartmentId { get; private set; }
    public string GarageNumber { get; private set; } = string.Empty;
    public GarageType Type { get; private set; }
    public Guid? OwnerId { get; private set; }
    public string? Description { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    // Navigation
    public Apartment Apartment { get; private set; } = null!;
    public Owner? Owner { get; private set; }

    public static Garage Create(
        Guid apartmentId,
        string garageNumber,
        GarageType type,
        string? description = null)
    {
        var garage = new Garage(
            Guid.NewGuid(),
            apartmentId,
            garageNumber,
            type)
        {
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        garage.RaiseDomainEvent(new GarageCreatedDomainEvent(garage.Id, apartmentId, garageNumber, type));

        return garage;
    }

    public void Update(GarageType type, string? description)
    {
        var oldType = Type;
        Type = type;
        Description = description;
        UpdatedAt = DateTime.UtcNow;

        if (oldType != type)
        {
            RaiseDomainEvent(new GarageTypeChangedDomainEvent(Id, oldType, type));
        }
    }

    public void AssignOwner(Guid ownerId)
    {
        var previousOwnerId = OwnerId;
        OwnerId = ownerId;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new GarageOwnerAssignedDomainEvent(Id, ownerId, previousOwnerId));
    }

    public void RemoveOwner()
    {
        var previousOwnerId = OwnerId;
        OwnerId = null;
        UpdatedAt = DateTime.UtcNow;

        if (previousOwnerId.HasValue)
        {
            RaiseDomainEvent(new GarageOwnerRemovedDomainEvent(Id, previousOwnerId.Value));
        }
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
        RaiseDomainEvent(new GarageDeletedDomainEvent(Id));
    }

    /// <summary>
    /// Premium qaraj/anbar üçün əlavə tərifə tətbiq olunur
    /// </summary>
    public bool IsPremium() => Type != GarageType.OpenParking;
}
