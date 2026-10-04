using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Garages;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Garages.Commands.SyncGarage;

internal sealed class SyncGarageCommandHandler : ICommandHandler<SyncGarageCommand>
{
    private readonly IGarageRepository _garageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncGarageCommandHandler(IGarageRepository garageRepository, IUnitOfWork unitOfWork)
    {
        _garageRepository = garageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SyncGarageCommand request, CancellationToken cancellationToken)
    {
        var garage = await _garageRepository.GetByIdDefaultAsync(request.GarageId, cancellationToken);
        if (garage is not null)
        {
            garage.UpdateDetails(request.GarageNumber, request.GarageType);
        }
        else
        {
            garage = Garage.Create(request.GarageId, request.GarageNumber, request.GarageType);
            _garageRepository.Add(garage);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
