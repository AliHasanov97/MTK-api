using MTK.Modules.Buildings.Domain.Appeals;
using MTK.Modules.Buildings.Domain.ValueObjects;
using Xunit;

namespace MTK.Tests.Buildings;

public sealed class AppealTests
{
    [Fact]
    public void Create_TrimsTextAndStartsSubmitted()
    {
        var submitterId = Guid.NewGuid();

        var appeal = Appeal.Create("  Water leak  ", "  Please repair  ", AppealCategory.Technical, submitterId);

        Assert.Equal("Water leak", appeal.Title);
        Assert.Equal("Please repair", appeal.Description);
        Assert.Equal(AppealStatus.Submitted, appeal.Status);
        Assert.Equal(AppealPriority.Normal, appeal.Priority);
        Assert.Equal(submitterId, appeal.SubmitterId);
    }

    [Fact]
    public void Create_WithBlankTitle_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Appeal.Create(" ", "Description", AppealCategory.General, Guid.NewGuid()));
    }

    [Fact]
    public void Resolve_SetsResolvedTimestamp()
    {
        var appeal = Appeal.Create("Issue", "Details", AppealCategory.General, Guid.NewGuid());

        appeal.UpdateStatus(AppealStatus.Resolved, Guid.NewGuid());

        Assert.Equal(AppealStatus.Resolved, appeal.Status);
        Assert.NotNull(appeal.ResolvedAt);
    }

    [Fact]
    public void Address_GetFullAddress_OmitsEmptyOptionalParts()
    {
        var address = new Address("Nizami 1", "Baku", country: null);

        Assert.Equal("Nizami 1, Baku", address.GetFullAddress());
    }
}
