using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.PropertyOwnerships.Commands.SyncPropertyOwnership;

internal sealed class SyncPropertyOwnershipCommandHandler(
    IPropertyOwnershipRepository propertyOwnershipRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<SyncPropertyOwnershipCommand>
{
    public async Task<Result> Handle(
        SyncPropertyOwnershipCommand request,
        CancellationToken cancellationToken)
    {
        Guid propertyId = request.ApartmentId ?? request.GarageId
            ?? throw new ArgumentException("Exactly one of ApartmentId/GarageId must be set", nameof(request));

        PropertyOwnership? propertyOwnership = await propertyOwnershipRepository
            .GetByPropertyIdAsync(propertyId, cancellationToken);

        if (propertyOwnership is not null)
        {
            propertyOwnership.UpdateOwner(request.OwnerId);
        }
        else
        {
            propertyOwnership = request.ApartmentId is { } apartmentId
                ? PropertyOwnership.CreateForApartment(apartmentId, request.OwnerId)
                : PropertyOwnership.CreateForGarage(request.GarageId!.Value, request.OwnerId);

            propertyOwnershipRepository.Add(propertyOwnership);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
