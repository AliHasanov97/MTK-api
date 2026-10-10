using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.DeleteEducationLeaveApplication;

public sealed record DeleteEducationLeaveApplicationCommand(Guid Id) : ICommand;
