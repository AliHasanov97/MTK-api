using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.SalaryPayments.Events;

/// <summary>
/// Domain event - maaş ödənişi Paid statusuna keçdikdə raise olunur.
/// Transaction-ların yaradılması üçün handler tətbiq olunur.
/// </summary>
public sealed record SalaryPaymentPaidDomainEvent(
    Guid SalaryPaymentId,
    Guid EmployeeId,
    int SalaryMonth,
    int SalaryYear,
    decimal GrossSalary) : DomainEvent;
