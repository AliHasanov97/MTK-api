using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;
using MTK.Modules.Hr.Domain.VacationReturnApplications;
using Xunit;

namespace MTK.Tests.Hr;

public sealed class LeaveApplicationTests
{
    [Fact]
    public void UnpaidLeaveApplication_RejectsEndDateBeforeStartDate()
    {
        var start = DateTimeOffset.UtcNow.Date.AddDays(5);
        var result = UnpaidLeaveApplication.Create(
            Guid.NewGuid(), start, start.AddDays(-1), Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("UnpaidLeaveApplication.InvalidDates", result.Error.Code);
    }

    [Fact]
    public void UnpaidLeaveApplication_RejectsPastStartDate()
    {
        var yesterday = DateTimeOffset.UtcNow.Date.AddDays(-1);
        var result = UnpaidLeaveApplication.Create(
            Guid.NewGuid(), yesterday, yesterday.AddDays(1), Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("UnpaidLeaveApplication.StartDateInPast", result.Error.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void VacationCompensationApplication_RejectsNonPositiveDays(int days)
    {
        var result = VacationCompensationApplication.Create(
            Guid.NewGuid(), Guid.NewGuid(), days, Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(VacationCompensationApplicationErrors.InvalidRequestedDays, result.Error);
    }

    [Fact]
    public void VacationCompensationApplication_ValidRequestCanUpdateNotes()
    {
        var result = VacationCompensationApplication.Create(
            Guid.NewGuid(), Guid.NewGuid(), 4, Guid.NewGuid(), "Initial");

        Assert.True(result.IsSuccess);
        Assert.Equal(ApplicationStatus.PendingApproval, result.Value.Status);
        result.Value.UpdateNotes("Updated");
        Assert.Equal("Updated", result.Value.Notes);
    }

    [Fact]
    public void VacationReturnApplication_CanOnlyChangeDateBeforeConversion()
    {
        var result = VacationReturnApplication.Create(
            Guid.NewGuid(), new DateTimeOffset(2027, 5, 1, 14, 0, 0, TimeSpan.FromHours(4)), Guid.NewGuid());
        var application = result.Value;

        Assert.True(application.UpdateReturnDate(new DateTimeOffset(2027, 5, 2, 0, 0, 0, TimeSpan.Zero)).IsSuccess);
        application.MarkAsConvertedToOrder();
        Assert.True(application.UpdateReturnDate(new DateTimeOffset(2027, 5, 3, 0, 0, 0, TimeSpan.Zero)).IsFailure);
    }
}
