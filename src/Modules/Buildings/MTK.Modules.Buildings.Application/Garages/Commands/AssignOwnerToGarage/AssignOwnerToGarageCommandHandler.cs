using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.Garages;

namespace MTK.Modules.Buildings.Application.Garages.Commands.AssignOwnerToGarage;

internal sealed class AssignOwnerToGarageCommandHandler
    : ICommandHandler<AssignOwnerToGarageCommand>
{
    private readonly IGarageRepository _garageRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;

    public AssignOwnerToGarageCommandHandler(
        IGarageRepository garageRepository,
        IOwnerRepository ownerRepository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _garageRepository = garageRepository;
        _ownerRepository = ownerRepository;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        AssignOwnerToGarageCommand request,
        CancellationToken cancellationToken)
    {
        var garage = await _garageRepository.GetByIdAsync(
            request.GarageId,
            cancellationToken);

        if (garage is null)
        {
            return Result.Failure(new Error(
                "Garage.NotFound",
                $"Qaraj tapılmadı: {request.GarageId}"));
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

        var previousOwnerId = garage.OwnerId;
        garage.AssignOwner(request.OwnerId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var integrationEvent = new GarageOwnerChangedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            request.GarageId,
            request.OwnerId,
            previousOwnerId);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        return Result.Success();
    }
}
