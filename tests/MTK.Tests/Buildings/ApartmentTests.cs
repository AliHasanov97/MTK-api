using MTK.Modules.Buildings.Domain.Apartments;
using MTK.Modules.Buildings.Domain.Enums;
using Xunit;

namespace MTK.Tests.Buildings;

public sealed class ApartmentTests
{
    [Fact]
    public void Create_StartsVacantAndStoresDetails()
    {
        var buildingId = Guid.NewGuid();

        var apartment = Apartment.Create(buildingId, "12A", 3, 84.5m, 3);

        Assert.Equal(buildingId, apartment.BuildingId);
        Assert.Equal("12A", apartment.ApartmentNumber);
        Assert.Equal(3, apartment.Floor);
        Assert.Equal(84.5m, apartment.AreaSquareMeters);
        Assert.Equal(3, apartment.RoomCount);
        Assert.Equal(ApartmentStatus.Vacant, apartment.Status);
    }

    [Fact]
    public void AssignAndRemoveOwner_UpdatesOwnerAndVacancyStatus()
    {
        var apartment = Apartment.Create(Guid.NewGuid(), "12A", 3, 84.5m, 3);
        var ownerId = Guid.NewGuid();

        apartment.AssignOwner(ownerId);

        Assert.Equal(ownerId, apartment.CurrentOwnerId);
        Assert.Equal(ApartmentStatus.Active, apartment.Status);

        apartment.RemoveOwner();

        Assert.Null(apartment.CurrentOwnerId);
        Assert.Equal(ApartmentStatus.Vacant, apartment.Status);
    }

    [Fact]
    public void MarkInTransfer_SetsTransferStatus()
    {
        var apartment = Apartment.Create(Guid.NewGuid(), "12A", 3, 84.5m, 3);
        apartment.AssignOwner(Guid.NewGuid());

        apartment.MarkInTransfer();

        Assert.Equal(ApartmentStatus.InTransfer, apartment.Status);
    }
}
