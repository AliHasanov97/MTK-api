using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Enums;
using MTK.Modules.Buildings.Domain.Garages;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Garages.Commands.CreateGarage;

internal sealed class CreateGarageCommandHandler : ICommandHandler<CreateGarageCommand, Guid>
{
    private readonly IGarageRepository _garageRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGarageCommandHandler(
        IGarageRepository garageRepository,
        IOwnerRepository ownerRepository,
        IUnitOfWork unitOfWork)
    {
        _garageRepository = garageRepository;
        _ownerRepository = ownerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateGarageCommand request, CancellationToken cancellationToken)
    {
        if (request.OwnerId.HasValue &&
            await _ownerRepository.GetByIdAsync(request.OwnerId.Value, cancellationToken) is null)
        {
            return Result.Failure<Guid>(new Error("Owner.NotFound", $"Sahib tapılmadı: {request.OwnerId}"));
        }

        if (await _garageRepository.ExistsByNumberAsync(request.GarageNumber, cancellationToken))
        {
            return Result.Failure<Guid>(new Error("Garage.AlreadyExists", $"Bu qaraj nömrəsi mövcuddur: {request.GarageNumber}"));
        }

        if (!Enum.TryParse<GarageType>(request.GarageType, out var garageType))
        {
            return Result.Failure<Guid>(new Error("GarageType.Invalid", $"Yanlış qaraj növü: {request.GarageType}"));
        }

        var garage = Garage.Create(request.OwnerId, request.GarageNumber, garageType, request.Description);
        _garageRepository.Add(garage);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        // Domain event handler will publish integration event

        return Result.Success(garage.Id);
    }
}
