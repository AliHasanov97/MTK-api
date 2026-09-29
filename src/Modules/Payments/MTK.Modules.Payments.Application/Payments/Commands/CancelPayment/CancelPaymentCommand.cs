using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payments.Commands.CancelPayment;

public sealed record CancelPaymentCommand(Guid PaymentId) : ICommand;
