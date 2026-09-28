using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Apartments.Events;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.Apartments;

namespace MTK.Modules.Buildings.Application.Apartments.Events;

internal sealed class ApartmentUpdatedDomainEventHandler : DomainEventHandler<ApartmentUpdatedDomainEvent>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IEventBus _eventBus;

    public ApartmentUpdatedDomainEventHandler(
        IApartmentRepository apartmentRepository,
        IEventBus eventBus)
    {
        _apartmentRepository = apartmentRepository;
        _eventBus = eventBus;
    }

    public override async Task Handle(
        ApartmentUpdatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // Get apartment to retrieve full details
        var apartment = await _apartmentRepository.GetByIdAsync(
            domainEvent.ApartmentId,
            cancellationToken);

        if (apartment is null)
        {
            return; // Apartment not found, skip
        }

        // Only sync if apartment has owner
        if (apartment.CurrentOwnerId.HasValue)
        {
            // Publish integration event to other modules
            var integrationEvent = new ApartmentUpdatedIntegrationEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                apartment.Id,
                apartment.AreaSquareMeters);

            await _eventBus.PublishAsync(integrationEvent, cancellationToken);
        }
    }
}
