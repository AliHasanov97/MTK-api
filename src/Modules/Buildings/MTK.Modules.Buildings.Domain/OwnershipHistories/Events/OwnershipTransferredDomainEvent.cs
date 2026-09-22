using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.OwnershipHistories.Events;

/// <summary>
/// Mülkiyyət transfer eventi - bu event digər module-lara bildiriş göndərir
/// Xüsusilə Billing module üçün mühümdür (borc yoxlama, faktura transfer)
/// </summary>
public sealed record OwnershipTransferredDomainEvent(
    Guid OwnershipHistoryId,
    Guid ApartmentId,
    Guid? PreviousOwnerId,
    Guid NewOwnerId,
    DateTime TransferDate) : DomainEvent;
