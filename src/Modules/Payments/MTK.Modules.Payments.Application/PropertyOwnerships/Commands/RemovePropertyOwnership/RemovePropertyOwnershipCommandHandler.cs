using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.PropertyOwnerships.Commands.RemovePropertyOwnership;

internal sealed class RemovePropertyOwnershipCommandHandler(
    IPropertyOwnershipRepository propertyOwnershipRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RemovePropertyOwnershipCommand>
{
    public async Task<Result> Handle(
        RemovePropertyOwnershipCommand request,
        CancellationToken cancellationToken)
    {
        PropertyOwnership? propertyOwnership = await propertyOwnershipRepository
            .GetByPropertyIdAsync(request.PropertyId, cancellationToken);

        if (propertyOwnership is null)
        {
            return Result.Success(); // Already removed or never existed
        }

        await propertyOwnershipRepository.DeleteAsync(
            propertyOwnership,
            null,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
