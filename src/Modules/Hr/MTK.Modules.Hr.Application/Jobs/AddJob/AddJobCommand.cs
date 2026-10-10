using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.Jobs.AddJob;

public sealed class AddJobCommand : ICommand<AddJobResponse>
{
    public string Name { get; set; } = string.Empty;
}
