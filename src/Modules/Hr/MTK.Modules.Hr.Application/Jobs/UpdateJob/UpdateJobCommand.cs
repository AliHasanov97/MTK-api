using MTK.Common.Application.Messaging;
using System.Text.Json.Serialization;

namespace MTK.Modules.Hr.Application.Jobs.UpdateJob;

public sealed class UpdateJobCommand : ICommand<UpdateJobResponse>
{
    [JsonIgnore]
    public Guid JobId { get; set; }
    public string Name { get; set; } = string.Empty;
}
