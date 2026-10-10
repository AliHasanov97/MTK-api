using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.DeleteUnpaidLeaveApplication;

public sealed record DeleteUnpaidLeaveApplicationCommand(Guid Id) : ICommand;
