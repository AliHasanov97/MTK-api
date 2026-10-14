using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payroll.Commands.RemovePayrollDeduction;

public sealed record RemovePayrollDeductionCommand(Guid SalaryDeductionId) : ICommand;
