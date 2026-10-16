using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.CompensationOrders.Events;

/// <summary>
/// Məzuniyyət kompensasiyası əmri yaradıldıqda qalxır. İstifadə olunmamış məzuniyyət
/// günlərinin pulla əvəzlənməsidir — işçiyə ödəniləcək məbləğ olduğu üçün Payments-ə
/// (əmək haqqı hesablamasına) ötürülməlidir.
/// </summary>
public sealed record CompensationOrderCreatedDomainEvent : DomainEvent
{
    public required Guid OrderId { get; init; }
    public required Guid EmployeeId { get; init; }
    public required int CompensatedDays { get; init; }
}
