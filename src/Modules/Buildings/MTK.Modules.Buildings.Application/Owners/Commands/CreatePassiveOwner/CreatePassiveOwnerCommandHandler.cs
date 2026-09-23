using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Owners.Commands.CreatePassiveOwner;

internal sealed class CreatePassiveOwnerCommandHandler : ICommandHandler<CreatePassiveOwnerCommand, Guid>
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePassiveOwnerCommandHandler(
        IOwnerRepository ownerRepository,
        IUnitOfWork unitOfWork)
    {
        _ownerRepository = ownerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreatePassiveOwnerCommand request,
        CancellationToken cancellationToken)
    {
        // Check if owner with this email already exists
        var existingOwner = await _ownerRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (existingOwner is not null)
        {
            return Result.Failure<Guid>(new Error(
                "Owner.EmailAlreadyExists",
                $"Owner with email '{request.Email}' already exists"));
        }

        // Create passive owner (no user account)
        var owner = Owner.CreatePassive(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Email,
            request.Notes);

        _ownerRepository.Add(owner);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(owner.Id);
    }
}
