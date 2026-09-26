using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Garages.Queries.Common;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Garages.Queries.GetGarageById;

internal sealed class GetGarageByIdQueryHandler
    : IQueryHandler<GetGarageByIdQuery, GarageResponse>
{
    private readonly IGarageRepository _garageRepository;

    public GetGarageByIdQueryHandler(IGarageRepository garageRepository)
    {
        _garageRepository = garageRepository;
    }

    public async Task<Result<GarageResponse>> Handle(
        GetGarageByIdQuery request,
        CancellationToken cancellationToken)
    {
        var garage = await _garageRepository.GetByIdAsync(
            request.GarageId,
            cancellationToken);

        if (garage is null)
        {
            return Result.Failure<GarageResponse>(new Error(
                "Garage.NotFound",
                $"Qaraj tapılmadı: {request.GarageId}"));
        }

        var ownerInfo = garage.Owner is not null && garage.OwnerId.HasValue
            ? new OwnerInfo(garage.OwnerId.Value, garage.Owner.FullName)
            : null;

        var response = new GarageResponse(
            garage.Id,
            garage.GarageNumber,
            garage.Type.ToString(),
            garage.Description,
            ownerInfo,
            garage.CreatedAt,
            garage.UpdatedAt);

        return Result.Success(response);
    }
}
