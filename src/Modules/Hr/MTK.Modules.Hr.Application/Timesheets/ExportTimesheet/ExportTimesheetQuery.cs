using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.Timesheets.ExportTimesheet;

public sealed class ExportTimesheetQuery : IQuery<ExportTimesheetResponse>
{
    public int Year { get; set; }
    public int Month { get; set; }
}

public sealed class ExportTimesheetResponse
{
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
