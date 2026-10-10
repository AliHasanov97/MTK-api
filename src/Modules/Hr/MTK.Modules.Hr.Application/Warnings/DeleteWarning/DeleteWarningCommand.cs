using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.Warnings.DeleteWarning;

public sealed class DeleteWarningCommand : ICommand
{
    public Guid Id { get; set; }
}
