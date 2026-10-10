using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.Employees.DeleteEmployee;

public sealed record DeleteEmployeeCommand(Guid Id) : ICommand;
