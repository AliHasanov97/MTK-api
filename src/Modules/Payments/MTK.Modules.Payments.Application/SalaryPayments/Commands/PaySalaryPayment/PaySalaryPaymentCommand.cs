using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.SalaryPayments;

namespace MTK.Modules.Payments.Application.SalaryPayments.Commands.PaySalaryPayment;

/// <summary>
/// Maaş ödənişini Paid statusuna keçir və Transaction-lar yaradır.
/// Bu əməliyyat geri qaytarıla bilməz.
/// </summary>
public sealed record PaySalaryPaymentCommand(Guid SalaryPaymentId) : ICommand<SalaryPaymentResponse>;
