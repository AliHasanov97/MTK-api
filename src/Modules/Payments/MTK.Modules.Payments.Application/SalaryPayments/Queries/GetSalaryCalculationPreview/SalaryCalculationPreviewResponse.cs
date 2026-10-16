namespace MTK.Modules.Payments.Application.SalaryPayments.Queries.GetSalaryCalculationPreview;

/// <summary>
/// Maaş hesablaması preview response.
/// </summary>
public sealed record SalaryCalculationPreviewResponse(
    Guid EmployeeId,
    string EmployeeFullName,
    int SalaryMonth,
    int SalaryYear,
    decimal BaseSalary,
    IReadOnlyCollection<PreviewBonusItem> Bonuses,
    IReadOnlyCollection<PreviewAdjustmentItem> Adjustments,
    decimal TotalBonuses,
    decimal TotalAdjustments,
    decimal GrossSalary);

/// <summary>
/// Bonus preview item.
/// </summary>
public sealed record PreviewBonusItem(
    Guid BonusId,
    string Description,
    decimal Amount);

/// <summary>
/// Adjustment preview item.
/// </summary>
public sealed record PreviewAdjustmentItem(
    Guid AdjustmentId,
    string Description,
    decimal Hours,
    decimal NormHours,
    decimal Amount);
