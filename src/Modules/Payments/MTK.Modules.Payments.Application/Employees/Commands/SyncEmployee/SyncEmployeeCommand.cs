using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Employees.Commands.SyncEmployee;

public sealed record SyncEmployeeCommand(Guid EmployeeId, string FullName, decimal BaseSalary) : ICommand;
