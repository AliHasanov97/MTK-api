using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payroll.Commands.UpdatePayrollDeduction;

public sealed record UpdatePayrollDeductionCommand(
    Guid SalaryDeductionId,
    decimal StateFeeRemaining,
    decimal DebtRemaining) : ICommand;
