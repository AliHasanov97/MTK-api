using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.VacationApplications;
using Xunit;

namespace MTK.Tests.Hr;

public sealed class VacationApplicationTests
{
    [Fact]
    public void Create_SetsPendingStatusAndDateOnlyValues()
    {
        var start = new DateTimeOffset(2027, 3, 4, 16, 30, 0, TimeSpan.FromHours(4));
        var end = new DateTimeOffset(2027, 3, 10, 9, 15, 0, TimeSpan.FromHours(4));

        var result = VacationApplication.Create(
            Guid.NewGuid(), Guid.NewGuid(), start, end, 5, Guid.NewGuid(), "Annual leave");

        Assert.True(result.IsSuccess);
        Assert.Equal(ApplicationStatus.PendingApproval, result.Value.Status);
        Assert.Equal(ApplicationType.Vacation, result.Value.Type);
        Assert.Equal(new DateTimeOffset(2027, 3, 4, 0, 0, 0, TimeSpan.Zero), result.Value.StartDate);
        Assert.Equal(new DateTimeOffset(2027, 3, 10, 0, 0, 0, TimeSpan.Zero), result.Value.EndDate);
        Assert.Equal(5, result.Value.TotalRequestedDays);
    }

    [Fact]
    public void Create_WhenEndBeforeStart_ReturnsValidationFailure()
    {
        var result = VacationApplication.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            new DateTimeOffset(2027, 3, 10, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2027, 3, 9, 0, 0, 0, TimeSpan.Zero),
            2, Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("VacationApplication.InvalidDates", result.Error.Code);
    }

    [Fact]
    public void UpdateDates_AfterConversion_IsRejected()
    {
        var application = VacationApplication.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            new DateTimeOffset(2027, 3, 4, 0, 0, 0, TimeSpan.Zero),
            null, 5, Guid.NewGuid()).Value;
        application.MarkAsConvertedToOrder();

        var result = application.UpdateDates(
            new DateTimeOffset(2027, 4, 1, 0, 0, 0, TimeSpan.Zero), null, 3);

        Assert.True(result.IsFailure);
        Assert.Equal(5, application.TotalRequestedDays);
    }
}
