using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.OwnerBalances.Queries.GetPropertyBalance;

public sealed record GetPropertyBalanceQuery(Guid PropertyId) : IQuery<PropertyBalanceResponse>;

/// <summary>
/// The net position for a single apartment/garage, from its own charges only.
/// By design a property can never carry a positive balance (an "advance") — any
/// payment surplus becomes owner-level advance instead (see GetOwnerBalance),
/// which SettleChargeFromAdvanceAsync automatically spends down the next time a
/// charge is created for any of the owner's properties.
/// </summary>
public sealed record PropertyBalanceResponse(
    Guid PropertyId,
    decimal TotalDebt,
    decimal TotalPaid,
    decimal CurrentBalance);
