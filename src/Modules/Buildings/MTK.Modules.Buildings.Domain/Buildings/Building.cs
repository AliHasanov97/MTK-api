using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Enums;
using MTK.Modules.Buildings.Domain.ValueObjects;
using MTK.Modules.Buildings.Domain.Apartments;
using MTK.Modules.Buildings.Domain.Buildings.Events;

namespace MTK.Modules.Buildings.Domain.Buildings;

/// <summary>
/// Bina entity - MTK-nin idarə etdiyi binalardan biri
/// </summary>
public sealed class Building : Entity
{
    private readonly List<Apartment> _apartments = new();

    private Building(
        Guid id,
        string name,
        Address address,
        int totalFloors,
        int apartmentsPerFloor) : base(id)
    {
        Name = name;
        Address = address;
        TotalFloors = totalFloors;
        ApartmentsPerFloor = apartmentsPerFloor;
        Status = BuildingStatus.Active;
    }

    // Private constructor for EF Core
    private Building() : base(Guid.Empty)
    {
    }

    public string Name { get; private set; } = string.Empty;
    public Address Address { get; private set; } = null!;
    public int TotalFloors { get; private set; }
    public int ApartmentsPerFloor { get; private set; }
    public int TotalApartments => TotalFloors * ApartmentsPerFloor;
    public BuildingStatus Status { get; private set; }
    public string? Description { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    // Navigation
    public IReadOnlyCollection<Apartment> Apartments => _apartments.AsReadOnly();

    public static Building Create(
        string name,
        Address address,
        int totalFloors,
        int apartmentsPerFloor,
        string? description = null)
    {
        var building = new Building(
            Guid.NewGuid(),
            name,
            address,
            totalFloors,
            apartmentsPerFloor)
        {
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        building.RaiseDomainEvent(new BuildingCreatedDomainEvent(building.Id, building.Name));

        return building;
    }

    public void Update(string name, Address address, string? description)
    {
        Name = name;
        Address = address;
        Description = description;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new BuildingUpdatedDomainEvent(Id, name));
    }

    public void UpdateStatus(BuildingStatus status)
    {
        var oldStatus = Status;
        Status = status;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new BuildingStatusChangedDomainEvent(Id, oldStatus, status));
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
        RaiseDomainEvent(new BuildingDeletedDomainEvent(Id));
    }
}
