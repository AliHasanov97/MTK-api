namespace MTK.Modules.Payments.Domain.Charges;

/// <summary>İllik hesabat üçün borcun yalnız lazımi sahələri (tam Charge entity-sini yükləmədən).</summary>
public sealed record OwnerChargeYearItem(
    Guid PropertyId,
    string Period,
    decimal Amount,
    decimal PaidAmount,
    ChargeStatus Status);
