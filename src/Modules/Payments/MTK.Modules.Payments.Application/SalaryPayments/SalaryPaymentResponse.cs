using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.SalaryPayments;

namespace MTK.Modules.Payments.Application.SalaryPayments;

/// <summary>
/// Maaş ödənişi response - detallı breakdown ilə.
/// </summary>
public sealed record SalaryPaymentResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeFullName,
    int SalaryMonth,
    int SalaryYear,
    decimal BaseSalary,
    decimal TotalBonuses,
    decimal TotalAdjustments,
    decimal GrossSalary,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    SalaryPaymentStatus Status,
    string? Notes,
    IReadOnlyCollection<SalaryPaymentLineResponse> Lines,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

/// <summary>
/// Maaş ödənişi line response.
/// </summary>
public sealed record SalaryPaymentLineResponse(
    Guid Id,
    SalaryPaymentLineType LineType,
    string Description,
    decimal Amount,
    Guid? SourceBonusId,
    Guid? SourceAdjustmentId);
