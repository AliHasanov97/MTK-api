using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.EmployeeWorkSchedules.SetEmployeeWorkSchedule;

public sealed class SetEmployeeWorkScheduleResponse
{
    public Guid Id { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateOnly EffectiveFrom { get; set; }
    public decimal? Monday { get; set; }
    public decimal? Tuesday { get; set; }
    public decimal? Wednesday { get; set; }
    public decimal? Thursday { get; set; }
    public decimal? Friday { get; set; }
    public decimal? Saturday { get; set; }
    public decimal? Sunday { get; set; }
}
