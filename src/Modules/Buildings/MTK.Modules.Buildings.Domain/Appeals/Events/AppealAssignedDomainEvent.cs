using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Appeals.Events;

/// <summary>
/// Müraciət məsul şəxsə təyin edildi.
/// </summary>
public sealed record AppealAssignedDomainEvent(
    Guid AppealId,
    Guid AssignedToId,
    Guid AssignedByUserId) : DomainEvent;
