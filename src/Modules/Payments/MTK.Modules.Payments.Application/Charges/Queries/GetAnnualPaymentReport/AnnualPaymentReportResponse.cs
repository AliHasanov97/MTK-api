using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;

public sealed record AnnualPaymentReportResponse(
    int Year,
    IReadOnlyCollection<PropertyAnnualReportRow> Properties);

public sealed record PropertyAnnualReportRow(
    Guid PropertyId,
    PropertyType PropertyType,
    IReadOnlyCollection<MonthlyChargeSummary> Months,
    // All-time outstanding (Amount - PaidAmount, cancelled excluded) for this
    // property — not scoped to the selected year, so it reflects today's real debt.
    decimal CurrentDebt);

/// <summary>
/// One calendar month's billing for a property. <see cref="Status"/> is null when
/// nothing active was billed that month (never charged, or every charge for it was
/// cancelled) — distinct from <see cref="ChargeStatus.Unpaid"/>, which means a real,
/// outstanding obligation exists.
/// </summary>
public sealed record MonthlyChargeSummary(
    int Month,
    decimal Amount,
    decimal PaidAmount,
    ChargeStatus? Status);
