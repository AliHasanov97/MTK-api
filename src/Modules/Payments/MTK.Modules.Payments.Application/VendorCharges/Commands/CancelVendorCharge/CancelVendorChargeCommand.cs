using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.VendorCharges.Commands.CancelVendorCharge;

/// <summary>
/// Səhv yaranmış borcu ləğv edir (silinmir — tarixçə qalır). Ödənilmiş borc
/// ləğv edilə bilməz.
/// </summary>
public sealed record CancelVendorChargeCommand(Guid ChargeId) : ICommand;
