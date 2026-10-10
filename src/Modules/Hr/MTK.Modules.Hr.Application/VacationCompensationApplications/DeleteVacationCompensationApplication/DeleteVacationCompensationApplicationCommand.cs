using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.DeleteVacationCompensationApplication;

public sealed record DeleteVacationCompensationApplicationCommand(Guid Id) : ICommand;
