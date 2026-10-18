using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Domain.VacationPays.Events;

/// <summary>
/// Məzuniyyət haqqı ödənildi.
/// Transaction yaradılması və şirkət balansından çıxılması üçün.
/// </summary>
public sealed record VacationPayPaidDomainEvent(
    Guid VacationPayId,
    Guid EmployeeId,
    decimal Amount,
    PaymentMethod PaymentMethod) : DomainEvent;
