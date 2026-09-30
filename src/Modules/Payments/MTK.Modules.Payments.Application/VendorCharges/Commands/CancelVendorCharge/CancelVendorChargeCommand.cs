using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.VendorCharges.Commands.CancelVendorCharge;

/// <summary>
/// Səhv yaranmış borcu ləğv edir (silinmir — tarixçə qalır). Ödənilmiş borc
/// ləğv edilə bilməz: əvvəlcə ödənişləri ləğv edilməlidir.
/// </summary>
public sealed record CancelVendorChargeCommand(
    Guid ChargeId,
    string? Reason = null) : ICommand;
