using MTK.Common.Application.Messaging;
using System.Text.Json.Serialization;

namespace MTK.Modules.Hr.Application.JobApplications.ConvertToEmploymentOrder;

public sealed class ConvertJobApplicationToEmploymentOrderCommand : ICommand<ConvertJobApplicationToEmploymentOrderResponse>
{
    [JsonIgnore]
    public Guid JobApplicationId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public Guid LaborCodeCaseId { get; set; }
}
