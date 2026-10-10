using MTK.Common.Application.Messaging;
using System.Text.Json.Serialization;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.UpdateApplicationForChangeOfPosition;

public sealed class UpdateApplicationForChangeOfPositionCommand : ICommand<UpdateApplicationForChangeOfPositionResponse>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public Guid? NewJobId { get; set; }
    public DateTimeOffset? SetDate { get; set; }
}
