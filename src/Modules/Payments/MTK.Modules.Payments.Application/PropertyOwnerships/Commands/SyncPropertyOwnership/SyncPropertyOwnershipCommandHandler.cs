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
        // Check if property ownership record exists
        PropertyOwnership? propertyOwnership = await propertyOwnershipRepository
            .GetByPropertyIdAsync(request.PropertyId, cancellationToken);

        if (propertyOwnership is not null)
        {
            // Update existing record
            propertyOwnership.UpdateOwner(request.OwnerId);

            if (request.AreaSquareMeters > 0)
            {
                propertyOwnership.UpdateArea(request.AreaSquareMeters);
            }
        }
        else
        {
            // Create new property ownership record
            propertyOwnership = PropertyOwnership.Create(
                request.PropertyId,
                request.PropertyType,
                request.OwnerId,
                request.AreaSquareMeters);

            propertyOwnershipRepository.Add(propertyOwnership);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
