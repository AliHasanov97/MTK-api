using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.SalaryDeductions.Commands.RecordPayment;

public sealed record RecordSalaryDeductionPaymentCommand(
    Guid EmployeeId,
    decimal Amount,
    int SalaryMonth,
    int SalaryYear) : ICommand;
