using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payroll;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payroll.Commands.PayPayslip;

/// <summary>Bir işçinin qəbzini ödənilmiş qeyd edir və Transaction (xərc) yaradır.</summary>
public sealed record PayPayslipCommand(
    Guid PayrollRunId,
    Guid EmployeeId,
    PaymentMethod PaymentMethod = PaymentMethod.BankTransfer) : ICommand<PayrollRunResponse>;
