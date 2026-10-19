using MTK.Modules.Hr.Domain.Helpers;
using Xunit;

namespace MTK.Tests.Hr;

public sealed class DateTimeHelperTests
{
    [Fact]
    public void ToUtcDateOnly_UsesInputCalendarDateAndRemovesTime()
    {
        var input = new DateTimeOffset(2026, 6, 7, 23, 45, 0, TimeSpan.FromHours(4));

        var result = DateTimeHelper.ToUtcDateOnly(input);

        Assert.Equal(new DateTimeOffset(2026, 6, 7, 0, 0, 0, TimeSpan.Zero), result);
    }

    [Fact]
    public void ToUtcDateOnly_NullableInputPreservesNull()
    {
        DateTimeOffset? input = null;

        Assert.Null(DateTimeHelper.ToUtcDateOnly(input));
    }
}
