using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories.DeleteEmployeeWorkHistory;

public sealed record DeleteEmployeeWorkHistoryCommand(Guid Id) : ICommand;
