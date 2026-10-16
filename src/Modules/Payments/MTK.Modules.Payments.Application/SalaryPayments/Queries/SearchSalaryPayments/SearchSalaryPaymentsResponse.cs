using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.SalaryPayments;

namespace MTK.Modules.Payments.Application.SalaryPayments.Queries.SearchSalaryPayments;

/// <summary>
/// Maaş ödənişləri axtarış response.
/// </summary>
public sealed record SearchSalaryPaymentsResponse(
    List<SalaryPaymentSearchResult> Items,
    int TotalCount,
    int Page,
    int PageSize);

/// <summary>
/// Maaş ödənişi axtarış nəticəsi (sadələşdirilmiş).
/// </summary>
public sealed record SalaryPaymentSearchResult(
    Guid Id,
    Guid EmployeeId,
    string EmployeeFullName,
    int SalaryMonth,
    int SalaryYear,
    decimal GrossSalary,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    SalaryPaymentStatus Status,
    DateTimeOffset CreatedAt);
