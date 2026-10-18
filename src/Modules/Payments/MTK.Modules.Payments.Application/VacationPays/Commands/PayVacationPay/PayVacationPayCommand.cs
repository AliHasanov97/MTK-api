using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.VacationPays.Commands.PayVacationPay;

/// <summary>
/// Məzuniyyət haqqını ödə.
/// Status Pending-dən Paid-ə keçir, transaction yaradır.
/// </summary>
public sealed record PayVacationPayCommand(
    Guid VacationPayId,
    PaymentMethod PaymentMethod) : ICommand;
