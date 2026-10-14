using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.CustomerPayments.Commands.CreateCustomerPayment;

/// <summary>
/// Müştəridən ödəniş qəbul edir. Ödəniş müştərinin açıq borclarına ən köhnədən (FIFO) paylanır, artıq qalıq
/// avans kimi qalır və növbəti borca avtomatik tətbiq olunur. Tamamlandıqda ledger-ə gəlir qeydi yazılır.
/// </summary>
public sealed record CreateCustomerPaymentCommand(
    Guid CustomerId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    string? Notes = null) : ICommand<Guid>;
