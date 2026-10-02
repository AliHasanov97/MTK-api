namespace MTK.Modules.Payments.Application.CompanyBalances.Queries.GetCompanyBalance;

/// <summary>
/// CurrentBalance is the company's real-time net cash position (all-time income
/// minus all-time expense) — "right now", not "as of a chosen past month". A
/// printed report for a specific month still needs to scan that month's ledger.
/// </summary>
public sealed record CompanyBalanceResponse(
    decimal TotalIncome,
    decimal TotalExpense,
    decimal CurrentBalance);
