using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Owners.Queries.GetOwnerById;

internal sealed class GetOwnerByIdQueryHandler
    : IQueryHandler<GetOwnerByIdQuery, OwnerResponse>
{
    private readonly IOwnerRepository _ownerRepository;

    public GetOwnerByIdQueryHandler(IOwnerRepository ownerRepository)
    {
        _ownerRepository = ownerRepository;
    }

    public async Task<Result<OwnerResponse>> Handle(
        GetOwnerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var owner = await _ownerRepository.GetByIdAsync(
            request.OwnerId,
            cancellationToken);

        if (owner is null)
        {
            return Result.Failure<OwnerResponse>(new Error(
                "Owner.NotFound",
                $"Sahib tapılmadı: {request.OwnerId}"));
        }

        var response = new OwnerResponse(
            owner.Id,
            owner.UserId,
            owner.FullName,
            owner.Email,
            owner.PhoneNumber,
            owner.IsActive,
            owner.OwnedApartments.Count);

        return Result.Success(response);
    }
}
