using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Garages.Commands.RemoveOwnerFromGarage;

internal sealed class RemoveOwnerFromGarageCommandHandler
    : ICommandHandler<RemoveOwnerFromGarageCommand>
{
    private readonly IGarageRepository _garageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveOwnerFromGarageCommandHandler(
        IGarageRepository garageRepository,
        IUnitOfWork unitOfWork)
    {
        _garageRepository = garageRepository;
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

        garage.RemoveOwner();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        // Domain event handler will publish integration event

        return Result.Success();
    }
}
