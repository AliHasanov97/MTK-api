using MTK.Modules.Buildings.Domain.Owners;
using Xunit;

namespace MTK.Tests.Buildings;

public sealed class OwnerTests
{
    [Fact]
    public void CreatePassive_HasNoUserAndIsUnregistered()
    {
        var owner = Owner.CreatePassive("Ali", "Mammadov", "+994501234567", "ali@example.com");

        Assert.Null(owner.UserId);
        Assert.False(owner.IsRegistered);
        Assert.True(owner.IsActive);
        Assert.Equal("Ali Mammadov", owner.FullName);
    }

    [Fact]
    public void LinkToUser_RegistersPassiveOwnerAndRejectsSecondLink()
    {
        var owner = Owner.CreatePassive("Ali", "Mammadov", "+994501234567", "ali@example.com");
        var userId = Guid.NewGuid();

        owner.LinkToUser(userId);

        Assert.Equal(userId, owner.UserId);
        Assert.True(owner.IsRegistered);
        Assert.Throws<InvalidOperationException>(() => owner.LinkToUser(Guid.NewGuid()));
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(0.01, true)]
    [InlineData(-1, false)]
    public void HasOutstandingDebt_ReturnsWhetherDebtIsPositive(decimal debt, bool expected)
    {
        var owner = Owner.CreatePassive("Ali", "Mammadov", "+994501234567", "ali@example.com");

        Assert.Equal(expected, owner.HasOutstandingDebt(debt));
    }
}
