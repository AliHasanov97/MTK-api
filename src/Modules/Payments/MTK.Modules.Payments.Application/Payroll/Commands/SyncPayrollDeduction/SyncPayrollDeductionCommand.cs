using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payroll.Commands.SyncPayrollDeduction;

public sealed record SyncPayrollDeductionCommand(
    Guid SalaryDeductionId,
    Guid EmployeeId,
    decimal PercentageSalary,
    DateTimeOffset StartDate) : ICommand;
