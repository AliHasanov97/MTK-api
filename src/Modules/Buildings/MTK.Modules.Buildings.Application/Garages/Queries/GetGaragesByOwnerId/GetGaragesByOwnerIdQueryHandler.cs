using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Garages.Queries.Common;
using MTK.Modules.Buildings.Application.Garages.Queries.GetGarageById;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Garages.Queries.GetGaragesByOwnerId;

internal sealed class GetGaragesByOwnerIdQueryHandler
    : IQueryHandler<GetGaragesByOwnerIdQuery, IEnumerable<GarageResponse>>
{
    private readonly IGarageRepository _garageRepository;

    public GetGaragesByOwnerIdQueryHandler(IGarageRepository garageRepository)
    {
        _garageRepository = garageRepository;
    }

    public async Task<Result<IEnumerable<GarageResponse>>> Handle(
        GetGaragesByOwnerIdQuery request,
        CancellationToken cancellationToken)
    {
        var garages = await _garageRepository.GetByOwnerIdAsync(
            request.OwnerId,
            cancellationToken);

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
