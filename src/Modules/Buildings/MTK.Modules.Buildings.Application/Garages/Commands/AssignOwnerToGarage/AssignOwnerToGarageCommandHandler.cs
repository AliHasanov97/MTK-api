using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Garages.Commands.AssignOwnerToGarage;

internal sealed class AssignOwnerToGarageCommandHandler
    : ICommandHandler<AssignOwnerToGarageCommand>
{
    private readonly IGarageRepository _garageRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignOwnerToGarageCommandHandler(
        IGarageRepository garageRepository,
        IOwnerRepository ownerRepository,
        IUnitOfWork unitOfWork)
    {
        _garageRepository = garageRepository;
        _ownerRepository = ownerRepository;
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

        garage.AssignOwner(request.OwnerId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        // Domain event handler will publish integration event

        return Result.Success();
    }
}
