using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.VendorPayments.Commands.CreateVendorPayment;

/// <summary>
/// Tədarükçüyə ödəniş edir. Sakin ödənişi ilə eyni məntiq: ödəniş tədarükçünün açıq
/// borclarına ən köhnədən (FIFO) paylanır, artıq qalıq isə avans kimi qalır və
/// növbəti borca tətbiq olunur. Tamamlandıqda ledger-ə xərc qeydi yazılır.
///
/// No client-supplied PaymentDate — the handler stamps it with DateTimeOffset.UtcNow.
/// </summary>
public sealed record CreateVendorPaymentCommand(
    Guid VendorId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    string? Notes = null) : ICommand<Guid>;
