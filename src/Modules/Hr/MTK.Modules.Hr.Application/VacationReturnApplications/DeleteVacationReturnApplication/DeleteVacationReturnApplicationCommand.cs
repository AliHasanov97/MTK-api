using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.DeleteVacationReturnApplication;

public sealed record DeleteVacationReturnApplicationCommand(Guid Id) : ICommand;