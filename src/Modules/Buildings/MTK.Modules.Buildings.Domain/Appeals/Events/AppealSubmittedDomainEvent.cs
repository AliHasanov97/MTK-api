using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Appeals.Events;

/// <summary>
/// Müraciət təqdim edildi (yeni yaradıldı).
/// </summary>
public sealed record AppealSubmittedDomainEvent(Guid AppealId, Guid SubmitterId) : DomainEvent;
