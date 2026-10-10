using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationApplications.DeleteVacationApplication;

public sealed record DeleteVacationApplicationCommand(Guid Id) : ICommand;