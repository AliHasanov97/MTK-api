using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Owners.Commands.UpdateOwner;

internal sealed class UpdateOwnerCommandHandler : ICommandHandler<UpdateOwnerCommand>
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOwnerCommandHandler(
        IOwnerRepository ownerRepository,
        IUnitOfWork unitOfWork)
    {
        _ownerRepository = ownerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateOwnerCommand request,
        CancellationToken cancellationToken)
    {
        var owner = await _ownerRepository.GetByUserIdAsync(
            request.UserId,
            cancellationToken);

        if (owner is null)
        {
            return Result.Success(); // Idempotent - owner yoxdursa skip
        }

        owner.UpdateContactInfo(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Email);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
