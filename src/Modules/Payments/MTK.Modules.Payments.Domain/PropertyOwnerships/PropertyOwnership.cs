using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Domain.PropertyOwnerships;

/// <summary>
/// Read model for property ownership data from Buildings module
/// Synchronized via integration events
/// </summary>
public sealed class PropertyOwnership : Entity
{
    private PropertyOwnership() : base() { }

    public Guid PropertyId { get; private set; }
    public PropertyType PropertyType { get; private set; }
    public Guid OwnerId { get; private set; }
    public decimal AreaSquareMeters { get; private set; }

    public static PropertyOwnership Create(
        Guid propertyId,
        PropertyType propertyType,
        Guid ownerId,
        decimal areaSquareMeters)
    {
        return new PropertyOwnership
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            PropertyType = propertyType,
            OwnerId = ownerId,
            AreaSquareMeters = areaSquareMeters
        };
    }

    public void UpdateOwner(Guid newOwnerId)
    {
        OwnerId = newOwnerId;
    }

    public void UpdateArea(decimal areaSquareMeters)
    {
        AreaSquareMeters = areaSquareMeters;
    }
}
