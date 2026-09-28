using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Apartments.Events;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.Apartments;

namespace MTK.Modules.Buildings.Application.Apartments.Events;

internal sealed class ApartmentOwnerAssignedDomainEventHandler : DomainEventHandler<ApartmentOwnerAssignedDomainEvent>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IEventBus _eventBus;

    public ApartmentOwnerAssignedDomainEventHandler(
        IApartmentRepository apartmentRepository,
        IEventBus eventBus)
    {
        _apartmentRepository = apartmentRepository;
        _eventBus = eventBus;
    }

    public override async Task Handle(
        ApartmentOwnerAssignedDomainEvent domainEvent,
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

        // Publish integration event to other modules
        var integrationEvent = new ApartmentOwnerChangedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            apartment.Id,
            domainEvent.NewOwnerId,
            domainEvent.PreviousOwnerId,
            apartment.AreaSquareMeters);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
