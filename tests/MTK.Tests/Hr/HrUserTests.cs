using MTK.Modules.Hr.Domain.Users;
using Xunit;

namespace MTK.Tests.Hr;

public sealed class HrUserTests
{
    [Fact]
    public void Update_ChangesProfileAndKeepsExistingIdentityWhenNoNewIdProvided()
    {
        var id = Guid.NewGuid();
        var user = User.Create(id, "Ali", "Mammadov", "ali@example.com", null, "identity-1");

        user.Update("Ali", "Aliyev", "new@example.com", "+994501234567");

        Assert.Equal(id, user.Id);
        Assert.Equal("Ali Aliyev", user.FullName);
        Assert.Equal("new@example.com", user.Email);
        Assert.Equal("identity-1", user.IdentityId);
    }

    [Fact]
    public void DeactivateAndActivate_ChangeStatus()
    {
        var user = User.Create(Guid.NewGuid(), "Ali", "Mammadov", "ali@example.com", null);

        user.Deactivate();
        Assert.Equal(UserStatus.Inactive, user.Status);

        user.Activate();
        Assert.Equal(UserStatus.Active, user.Status);
    }
}
