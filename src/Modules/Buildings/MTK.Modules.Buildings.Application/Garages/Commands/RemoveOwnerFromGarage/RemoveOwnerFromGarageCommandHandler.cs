using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.Garages;

namespace MTK.Modules.Buildings.Application.Garages.Commands.RemoveOwnerFromGarage;

internal sealed class RemoveOwnerFromGarageCommandHandler
    : ICommandHandler<RemoveOwnerFromGarageCommand>
{
    private readonly IGarageRepository _garageRepository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveOwnerFromGarageCommandHandler(
        IGarageRepository garageRepository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _garageRepository = garageRepository;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RemoveOwnerFromGarageCommand request,
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

        if (garage.OwnerId is null)
        {
            return Result.Failure(new Error(
                "Garage.NoOwner",
                "Qarajın sahibi yoxdur"));
        }

        var removedOwnerId = garage.OwnerId.Value;
        garage.RemoveOwner();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var integrationEvent = new GarageOwnerRemovedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            request.GarageId,
            removedOwnerId);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        return Result.Success();
    }
}
