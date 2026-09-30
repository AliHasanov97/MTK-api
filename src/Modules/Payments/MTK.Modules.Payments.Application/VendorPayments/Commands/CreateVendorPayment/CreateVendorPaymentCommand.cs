using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.VendorPayments.Commands.CreateVendorPayment;

/// <summary>
/// Tədarükçüyə ödəniş edir: borcu qismən/tam bağlayır (ApplyPayment) və
/// tamamlandıqda ledger-ə xərc qeydi yazılır (domain event → handler).
/// </summary>
public sealed record CreateVendorPaymentCommand(
    Guid VendorChargeId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    string? Reference = null,
    string? Notes = null) : ICommand<Guid>;
