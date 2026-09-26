using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Enums;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.Garages.Events;

namespace MTK.Modules.Buildings.Domain.Garages;

/// <summary>
/// Qaraj və ya dayanacaq entity
/// </summary>
public sealed class Garage : SearchableEntity
{
    private Garage(
        Guid id,
        Guid? ownerId,
        string garageNumber,
        GarageType type) : base(id)
    {
        OwnerId = ownerId;
        GarageNumber = garageNumber;
        Type = type;
    }

    // Private constructor for EF Core
    private Garage() : base()
    {
    }

    public string GarageNumber { get; private set; } = string.Empty;
    public GarageType Type { get; private set; }
    public Guid? OwnerId { get; private set; }
    public string? Description { get; private set; }

    // Navigation
    public Owner? Owner { get; private set; }

    public static Garage Create(
        Guid? ownerId,
        string garageNumber,
        GarageType type,
        string? description = null)
    {
        var garage = new Garage(
            Guid.NewGuid(),
            ownerId,
            garageNumber,
            type)
        {
            Description = description
        };

        garage.SetCreatedAt();
        garage.RaiseDomainEvent(new GarageCreatedDomainEvent(garage.Id, ownerId, garageNumber, type));

        return garage;
    }

    public void Update(GarageType type, string? description)
    {
        var oldType = Type;
        Type = type;
        Description = description;
        SetUpdatedAt();

        if (oldType != type)
        {
            RaiseDomainEvent(new GarageTypeChangedDomainEvent(Id, oldType, type));
        }
    }

    public void AssignOwner(Guid ownerId)
    {
        var previousOwnerId = OwnerId;
        OwnerId = ownerId;
        SetUpdatedAt();

        RaiseDomainEvent(new GarageOwnerAssignedDomainEvent(Id, ownerId, previousOwnerId));
    }

    public void RemoveOwner()
    {
        var previousOwnerId = OwnerId;
        OwnerId = null;
        SetUpdatedAt();

        if (previousOwnerId.HasValue)
        {
            RaiseDomainEvent(new GarageOwnerRemovedDomainEvent(Id, previousOwnerId.Value));
        }
    }

    public void Delete()
    {
        SetDeletedAt();
        RaiseDomainEvent(new GarageDeletedDomainEvent(Id));
    }

    /// <summary>
    /// Premium qaraj/anbar üçün əlavə tərifə tətbiq olunur
    /// </summary>
    public bool IsPremium() => Type != GarageType.OpenParking;
}
