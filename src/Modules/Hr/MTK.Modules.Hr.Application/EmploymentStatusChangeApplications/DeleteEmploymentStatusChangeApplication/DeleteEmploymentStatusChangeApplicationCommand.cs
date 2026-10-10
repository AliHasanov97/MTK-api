using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.DeleteEmploymentStatusChangeApplication;

public sealed record DeleteEmploymentStatusChangeApplicationCommand(Guid Id)
    : ICommand<DeleteEmploymentStatusChangeApplicationResponse>;
