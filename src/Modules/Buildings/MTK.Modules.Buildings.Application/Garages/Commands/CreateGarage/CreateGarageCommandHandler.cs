using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Enums;
using MTK.Modules.Buildings.Domain.Garages;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.Garages;

namespace MTK.Modules.Buildings.Application.Garages.Commands.CreateGarage;

internal sealed class CreateGarageCommandHandler
    : ICommandHandler<CreateGarageCommand, Guid>
{
    private readonly IGarageRepository _garageRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGarageCommandHandler(
        IGarageRepository garageRepository,
        IApartmentRepository apartmentRepository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _garageRepository = garageRepository;
        _apartmentRepository = apartmentRepository;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateGarageCommand request,
        CancellationToken cancellationToken)
    {
        var apartment = await _apartmentRepository.GetByIdAsync(
            request.ApartmentId,
            cancellationToken);

        if (apartment is null)
        {
            return Result.Failure<Guid>(new Error(
                "Apartment.NotFound",
                $"Mənzil tapılmadı: {request.ApartmentId}"));
        }

        var exists = await _garageRepository.ExistsByNumberAsync(
            request.GarageNumber,
            cancellationToken);

        if (exists)
        {
            return Result.Failure<Guid>(new Error(
                "Garage.AlreadyExists",
                $"Bu nömrəli qaraj artıq mövcuddur: {request.GarageNumber}"));
        }

        if (!Enum.TryParse<GarageType>(request.GarageType, out var garageType))
        {
            return Result.Failure<Guid>(new Error(
                "GarageType.Invalid",
                $"Yanlış qaraj növü: {request.GarageType}"));
        }

        var garage = Garage.Create(
            request.ApartmentId,
            request.GarageNumber,
            garageType,
            request.Description);

        _garageRepository.Add(garage);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var integrationEvent = new GarageCreatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            garage.Id,
            request.ApartmentId,
            request.GarageNumber,
            request.GarageType);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        return Result.Success(garage.Id);
    }
}
