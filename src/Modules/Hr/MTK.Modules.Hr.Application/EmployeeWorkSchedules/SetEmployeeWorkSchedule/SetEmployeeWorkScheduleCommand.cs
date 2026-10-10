using System.Text.Json.Serialization;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmployeeWorkSchedules.SetEmployeeWorkSchedule;

public sealed class SetEmployeeWorkScheduleCommand : ICommand<SetEmployeeWorkScheduleResponse>
{
    [JsonIgnore]
    public Guid EmployeeId { get; set; }

    // EffectiveFrom UI-dan gəlmir, server avtomatik bugünkü tarixi istifadə edir

    public decimal? Monday { get; set; }
    public decimal? Tuesday { get; set; }
    public decimal? Wednesday { get; set; }
    public decimal? Thursday { get; set; }
    public decimal? Friday { get; set; }
    public decimal? Saturday { get; set; }
    public decimal? Sunday { get; set; }
}
