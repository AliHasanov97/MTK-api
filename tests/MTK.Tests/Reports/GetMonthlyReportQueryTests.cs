using MTK.Modules.Reports.Application.MonthlyReports;
using Xunit;

namespace MTK.Tests.Reports;

public sealed class GetMonthlyReportQueryTests
{
    [Fact]
    public void Query_PreservesRequestedYearAndMonth()
    {
        var query = new GetMonthlyReportQuery(2026, 10);

        Assert.Equal(2026, query.Year);
        Assert.Equal(10, query.Month);
    }
}
