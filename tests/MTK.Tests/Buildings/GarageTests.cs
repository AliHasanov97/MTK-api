using MTK.Modules.Buildings.Domain.Enums;
using MTK.Modules.Buildings.Domain.Garages;
using Xunit;

namespace MTK.Tests.Buildings;

public sealed class GarageTests
{
    [Theory]
    [InlineData(GarageType.OpenParking, false)]
    [InlineData(GarageType.CoveredGarage, true)]
    [InlineData(GarageType.Storage, true)]
    public void IsPremium_DependsOnGarageType(GarageType type, bool expected)
    {
        var garage = Garage.Create(null, "G-01", type);

        Assert.Equal(expected, garage.IsPremium());
    }

    [Fact]
    public void AssignAndRemoveOwner_UpdatesOwner()
    {
        var garage = Garage.Create(null, "G-01", GarageType.CoveredGarage);
        var ownerId = Guid.NewGuid();

        garage.AssignOwner(ownerId);
        Assert.Equal(ownerId, garage.OwnerId);

        garage.RemoveOwner();
        Assert.Null(garage.OwnerId);
    }
}
