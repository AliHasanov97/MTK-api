using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.Timesheets.GetTimesheets;

public sealed class GetTimesheetsQuery : IQuery<GetTimesheetsResponse>
{
    public int Year { get; set; }
    public int Month { get; set; }
}
