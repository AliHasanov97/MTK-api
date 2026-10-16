using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.SalaryPayments.Commands.DeleteSalaryPayment;

/// <summary>
/// Maaş ödənişini sil (yalnız Draft statusunda).
/// </summary>
public sealed record DeleteSalaryPaymentCommand(Guid SalaryPaymentId) : ICommand;
