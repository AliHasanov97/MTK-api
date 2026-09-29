namespace MTK.Modules.Payments.Application.OwnerBalances.Queries.GetOwnerBalance;

/// <summary>
/// CurrentBalance is the single source of truth for the owner's advance/debt —
/// positive means credit sitting at the owner level (never on any one property;
/// see PropertyBalanceResponse), which SettleChargeFromAdvanceAsync automatically
/// spends down as soon as a new charge is created for any of their properties.
/// </summary>
public sealed record OwnerBalanceResponse(
    Guid Id,
    Guid OwnerId,
    decimal TotalDebt,
    decimal TotalPaid,
    decimal CurrentBalance);
