using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Apartments;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.Apartments;

namespace MTK.Modules.Buildings.Application.Apartments.Commands.CreateApartment;

internal sealed class CreateApartmentCommandHandler
    : ICommandHandler<CreateApartmentCommand, Guid>
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;

    public CreateApartmentCommandHandler(
        IApartmentRepository apartmentRepository,
        IBuildingRepository buildingRepository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _apartmentRepository = apartmentRepository;
        _buildingRepository = buildingRepository;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateApartmentCommand request,
        CancellationToken cancellationToken)
    {
        var building = await _buildingRepository.GetByIdAsync(
            request.BuildingId,
            cancellationToken);

        if (building is null)
        {
            return Result.Failure<Guid>(new Error(
                "Building.NotFound",
                $"Bina tapılmadı: {request.BuildingId}"));
        }

        var exists = await _apartmentRepository.ExistsByNumberAsync(
            request.BuildingId,
            request.ApartmentNumber,
            cancellationToken);

        if (exists)
        {
            return Result.Failure<Guid>(new Error(
                "Apartment.AlreadyExists",
                $"Bu nömrəli mənzil artıq mövcuddur: {request.ApartmentNumber}"));
        }

        var apartment = Apartment.Create(
            request.BuildingId,
            request.ApartmentNumber,
            request.Floor,
            request.AreaSquareMeters,
            request.RoomCount);

        _apartmentRepository.Add(apartment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var integrationEvent = new ApartmentCreatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            apartment.Id,
            request.BuildingId,
            request.ApartmentNumber,
            request.AreaSquareMeters);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        return Result.Success(apartment.Id);
    }
}
