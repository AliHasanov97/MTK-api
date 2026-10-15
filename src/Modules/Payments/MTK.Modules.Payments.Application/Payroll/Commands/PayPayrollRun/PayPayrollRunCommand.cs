using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payroll;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payroll.Commands.PayPayrollRun;

/// <summary>Hələ ödənilməmiş bütün qəbzləri ödənilmiş qeyd edir — hər biri üçün ayrıca Transaction yaradır.</summary>
public sealed record PayPayrollRunCommand(
    Guid PayrollRunId,
    PaymentMethod PaymentMethod = PaymentMethod.BankTransfer) : ICommand<PayrollRunResponse>;
