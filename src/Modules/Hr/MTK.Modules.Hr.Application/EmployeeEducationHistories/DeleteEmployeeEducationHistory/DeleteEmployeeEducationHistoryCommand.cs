using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmployeeEducationHistories.DeleteEmployeeEducationHistory;

public sealed record DeleteEmployeeEducationHistoryCommand(Guid Id) : ICommand;
