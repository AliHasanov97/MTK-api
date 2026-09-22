using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Owners.Commands.UpdateOwnerRole;

internal sealed class UpdateOwnerRoleCommandHandler : ICommandHandler<UpdateOwnerRoleCommand>
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOwnerRoleCommandHandler(
        IOwnerRepository ownerRepository,
        IUnitOfWork unitOfWork)
    {
        _ownerRepository = ownerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateOwnerRoleCommand request,
        CancellationToken cancellationToken)
    {
        var owner = await _ownerRepository.GetByUserIdAsync(
            request.UserId,
            cancellationToken);

        // User -> ApartmentOwner: Owner yarat
        if (request.NewRole == "ApartmentOwner" && owner is null)
        {
            var newOwner = Owner.Create(
                request.UserId,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Email);

            _ownerRepository.Add(newOwner);
        }
        // ApartmentOwner -> User: Owner deaktiv et
        else if (request.OldRole == "ApartmentOwner" && owner is not null)
        {
            owner.Deactivate();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
