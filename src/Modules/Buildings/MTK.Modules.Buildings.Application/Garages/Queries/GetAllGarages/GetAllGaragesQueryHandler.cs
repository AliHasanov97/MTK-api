using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Garages.Queries.Common;
using MTK.Modules.Buildings.Application.Garages.Queries.GetGarageById;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Garages.Queries.GetAllGarages;

internal sealed class GetAllGaragesQueryHandler
    : IQueryHandler<GetAllGaragesQuery, IEnumerable<GarageResponse>>
{
    private readonly IGarageRepository _garageRepository;

    public GetAllGaragesQueryHandler(IGarageRepository garageRepository)
    {
        _garageRepository = garageRepository;
    }

    public async Task<Result<IEnumerable<GarageResponse>>> Handle(
        GetAllGaragesQuery request,
        CancellationToken cancellationToken)
    {
        var garages = await _garageRepository.GetAllAsync(cancellationToken);

        var response = garages.Select(garage =>
        {
            var ownerInfo = garage.Owner is not null && garage.OwnerId.HasValue
                ? new OwnerInfo(garage.OwnerId.Value, garage.Owner.FullName)
                : null;

            return new GarageResponse(
                garage.Id,
                garage.GarageNumber,
                garage.Type.ToString(),
                garage.Description,
                ownerInfo,
                garage.CreatedAt,
                garage.UpdatedAt);
        });

        return Result.Success(response);
    }
}
