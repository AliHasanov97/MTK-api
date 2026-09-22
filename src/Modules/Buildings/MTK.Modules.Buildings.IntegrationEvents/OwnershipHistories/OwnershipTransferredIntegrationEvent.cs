using MTK.Common.Application.EventBus;

namespace MTK.Modules.Buildings.IntegrationEvents.OwnershipHistories;

/// <summary>
/// Buildings modulunda mülkiyyət transfer edildikdə digər module-lara bildiriş göndərmək üçün
/// Billing module bu event-i consume edib:
/// - Köhnə owner-in borclarını yoxlayır (transfer ancaq borc 0 olanda mümkündür)
/// - Yeni owner üçün yeni faturalar generasiya edir
/// </summary>
public sealed class OwnershipTransferredIntegrationEvent : IntegrationEvent
{
    public OwnershipTransferredIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid apartmentId,
        Guid? previousOwnerId,
        Guid newOwnerId,
        DateTime transferDate)
        : base(integrationEventId, occurredOnUtc)
    {
        ApartmentId = apartmentId;
        PreviousOwnerId = previousOwnerId;
        NewOwnerId = newOwnerId;
        TransferDate = transferDate;
    }

    public Guid ApartmentId { get; }
    public Guid? PreviousOwnerId { get; }
    public Guid NewOwnerId { get; }
    public DateTime TransferDate { get; }
}
