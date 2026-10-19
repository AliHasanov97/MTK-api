using MTK.Modules.Buildings.Domain.Buildings;
using MTK.Modules.Buildings.Domain.Enums;
using MTK.Modules.Buildings.Domain.ValueObjects;
using Xunit;

namespace MTK.Tests.Buildings;

public sealed class BuildingTests
{
    [Fact]
    public void Create_SetsActiveStatusAndAddress()
    {
        var address = new Address("Nizami 1", "Baku");

        var building = Building.Create("Tower A", address, "Main building");

        Assert.Equal("Tower A", building.Name);
        Assert.Equal(address, building.Address);
        Assert.Equal(BuildingStatus.Active, building.Status);
        Assert.Equal("Main building", building.Description);
    }

    [Fact]
    public void UpdateStatus_ChangesStatus()
    {
        var building = Building.Create("Tower A", new Address("Nizami 1", "Baku"));

        building.UpdateStatus(BuildingStatus.UnderMaintenance);

        Assert.Equal(BuildingStatus.UnderMaintenance, building.Status);
    }
}
