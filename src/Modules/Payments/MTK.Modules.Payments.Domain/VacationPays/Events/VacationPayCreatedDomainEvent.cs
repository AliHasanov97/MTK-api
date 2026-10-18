using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.VacationPays.Events;

/// <summary>
/// Məzuniyyət haqqı yaradıldı.
/// İndi heç bir handler yoxdur, gələcəkdə notification və s. əlavə oluna bilər.
/// </summary>
public sealed record VacationPayCreatedDomainEvent(Guid VacationPayId) : DomainEvent;
