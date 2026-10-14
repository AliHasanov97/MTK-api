using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.CustomerCharges.Commands.CancelCustomerCharge;

/// <summary>Səhv yaranmış müştəri borcunu ləğv edir (silinmir — tarixçə qalır). Ödənilmiş borc ləğv edilə bilməz.</summary>
public sealed record CancelCustomerChargeCommand(Guid ChargeId) : ICommand;
