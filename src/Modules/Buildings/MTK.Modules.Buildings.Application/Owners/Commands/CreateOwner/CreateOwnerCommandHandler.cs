using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Owners.Commands.CreateOwner;

internal sealed class CreateOwnerCommandHandler : ICommandHandler<CreateOwnerCommand>
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOwnerCommandHandler(
        IOwnerRepository ownerRepository,
        IUnitOfWork unitOfWork)
    {
        _ownerRepository = ownerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        CreateOwnerCommand request,
        CancellationToken cancellationToken)
    {
        // Check if Owner already exists
        var existingOwner = await _ownerRepository.GetByUserIdAsync(
            request.UserId,
            cancellationToken);

        if (existingOwner is not null)
        {
            return Result.Success(); // Idempotent
        }

        // Create new Owner
        var owner = Owner.Create(
            request.UserId,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Email);

        _ownerRepository.Add(owner);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
