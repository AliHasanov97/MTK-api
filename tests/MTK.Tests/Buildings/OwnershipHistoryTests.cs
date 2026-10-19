using MTK.Modules.Buildings.Domain.OwnershipHistories;
using Xunit;

namespace MTK.Tests.Buildings;

public sealed class OwnershipHistoryTests
{
    [Fact]
    public void CreateInitial_HasNoPreviousOwner()
    {
        var apartmentId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var transferDate = new DateTime(2026, 10, 19);

        var history = OwnershipHistory.CreateInitial(apartmentId, ownerId, "Ali Mammadov", transferDate);

        Assert.Equal(apartmentId, history.ApartmentId);
        Assert.Null(history.PreviousOwnerId);
        Assert.Null(history.PreviousOwnerName);
        Assert.Equal(ownerId, history.NewOwnerId);
        Assert.Equal("Ali Mammadov", history.NewOwnerName);
        Assert.Equal(transferDate, history.TransferDate);
    }
}
