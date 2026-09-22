using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.Apartments;

namespace MTK.Modules.Buildings.Application.Apartments.Commands.AssignOwnerToApartment;

internal sealed class AssignOwnerToApartmentCommandHandler
    : ICommandHandler<AssignOwnerToApartmentCommand>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;

    public AssignOwnerToApartmentCommandHandler(
        IApartmentRepository apartmentRepository,
        IOwnerRepository ownerRepository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _apartmentRepository = apartmentRepository;
        _ownerRepository = ownerRepository;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        AssignOwnerToApartmentCommand request,
        CancellationToken cancellationToken)
    {
        var apartment = await _apartmentRepository.GetByIdAsync(
            request.ApartmentId,
            cancellationToken);

        if (apartment is null)
        {
            return Result.Failure(new Error(
                "Apartment.NotFound",
                $"Mənzil tapılmadı: {request.ApartmentId}"));
        }

        var owner = await _ownerRepository.GetByIdAsync(
            request.OwnerId,
            cancellationToken);

        if (owner is null)
        {
            return Result.Failure(new Error(
                "Owner.NotFound",
                $"Sahib tapılmadı: {request.OwnerId}"));
        }

        var previousOwnerId = apartment.CurrentOwnerId;
        apartment.AssignOwner(request.OwnerId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var integrationEvent = new ApartmentOwnerChangedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            request.ApartmentId,
            request.OwnerId,
            previousOwnerId);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        return Result.Success();
    }
}
