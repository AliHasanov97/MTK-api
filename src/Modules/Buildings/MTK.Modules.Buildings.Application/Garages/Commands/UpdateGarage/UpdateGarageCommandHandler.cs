using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Garages.Commands.UpdateGarage;

internal sealed class UpdateGarageCommandHandler
    : ICommandHandler<UpdateGarageCommand>
{
    private readonly IGarageRepository _garageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGarageCommandHandler(
        IGarageRepository garageRepository,
        IUnitOfWork unitOfWork)
    {
        _garageRepository = garageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateGarageCommand request,
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

        garage.Update(request.Type, request.Description);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
