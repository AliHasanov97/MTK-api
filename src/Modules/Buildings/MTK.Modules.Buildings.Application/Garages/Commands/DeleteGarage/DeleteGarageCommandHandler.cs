using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Garages.Commands.DeleteGarage;

internal sealed class DeleteGarageCommandHandler
    : ICommandHandler<DeleteGarageCommand>
{
    private readonly IGarageRepository _garageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGarageCommandHandler(
        IGarageRepository garageRepository,
        IUnitOfWork unitOfWork)
    {
        _garageRepository = garageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteGarageCommand request,
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

        if (garage.DeletedAt is not null)
        {
            return Result.Failure(new Error(
                "Garage.AlreadyDeleted",
                "Qaraj artıq silinib"));
        }

        garage.Delete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
