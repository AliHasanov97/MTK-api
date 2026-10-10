using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.Jobs.DeleteJob;

public sealed record DeleteJobCommand(Guid JobId) : ICommand;
