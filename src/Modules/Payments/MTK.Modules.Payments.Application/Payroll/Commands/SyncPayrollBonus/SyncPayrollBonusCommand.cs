using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payroll.Commands.SyncPayrollBonus;

public sealed record SyncPayrollBonusCommand(
    Guid BonusOrderId,
    Guid EmployeeId,
    decimal BonusAmount,
    int SalaryMonth,
    int SalaryYear) : ICommand;
