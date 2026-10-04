using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Apartments;
using MTK.Modules.Payments.Domain.Garages;
using MTK.Modules.Payments.Domain.Owners;

namespace MTK.Modules.Payments.Domain.PropertyOwnerships;

/// <summary>
/// "Hansı əmlak kimə məxsusdur" — Buildings-dən sinxronlaşan mülkiyyət xəritəsi.
/// Əmlakın/sahibin öz təsviri (nömrə, sahə, qaraj tipi, bina) artıq burada
/// təkrarlanmır — <see cref="Apartment"/>/<see cref="Garage"/>/<see cref="Owner"/>
/// shadow-larının öz işidir; bu, yalnız onları <see cref="OwnerId"/> ilə bağlayır.
/// </summary>
public sealed class PropertyOwnership : Entity
{
    private PropertyOwnership() : base() { }

    public Guid? ApartmentId { get; private set; }
    public Apartment? Apartment { get; private set; }

    public Guid? GarageId { get; private set; }
    public Garage? Garage { get; private set; }

    public Guid OwnerId { get; private set; }
    public Owner? Owner { get; private set; }

    public static PropertyOwnership CreateForApartment(Guid apartmentId, Guid ownerId)
    {
        return new PropertyOwnership
        {
            Id = Guid.NewGuid(),
            ApartmentId = apartmentId,
            OwnerId = ownerId,
        };
    }

    public static PropertyOwnership CreateForGarage(Guid garageId, Guid ownerId)
    {
        return new PropertyOwnership
        {
            Id = Guid.NewGuid(),
            GarageId = garageId,
            OwnerId = ownerId,
        };
    }

    public void UpdateOwner(Guid newOwnerId)
    {
        OwnerId = newOwnerId;
    }
}
