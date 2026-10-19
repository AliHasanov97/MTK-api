using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.Appeals.Events;

/// <summary>
/// Müraciətin statusu dəyişdi.
/// </summary>
public sealed record AppealStatusChangedDomainEvent(
    Guid AppealId,
    AppealStatus OldStatus,
    AppealStatus NewStatus,
    Guid UpdatedByUserId) : DomainEvent;
