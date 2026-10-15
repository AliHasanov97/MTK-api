using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationReturnOrders.Events;

/// <summary>
/// Məzuniyyətdən geri qayıtma əmri yaradıldıqda qalxır. İşçi məzuniyyəti vaxtından əvvəl
/// kəsdiyi üçün qaytarılmış gün sayı (<see cref="UnusedDaysReturned"/>) bildirilir —
/// məzuniyyət ödənişi ilə əlaqəli düzəlişlərin (əgər varsa) yenidən hesablanması üçün.
/// </summary>
public sealed record VacationReturnOrderCreatedDomainEvent : DomainEvent
{
    public required Guid OrderId { get; init; }
    public required Guid EmployeeId { get; init; }
    public required DateTimeOffset ReturnDate { get; init; }
    public required int UnusedDaysReturned { get; init; }
}
