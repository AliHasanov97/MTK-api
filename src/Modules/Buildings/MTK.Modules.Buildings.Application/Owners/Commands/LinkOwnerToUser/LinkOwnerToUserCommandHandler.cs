using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Owners.Commands.LinkOwnerToUser;

internal sealed class LinkOwnerToUserCommandHandler : ICommandHandler<LinkOwnerToUserCommand>
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LinkOwnerToUserCommandHandler(
        IOwnerRepository ownerRepository,
        IUnitOfWork unitOfWork)
    {
        _ownerRepository = ownerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        LinkOwnerToUserCommand request,
        CancellationToken cancellationToken)
    {
        var owner = await _ownerRepository.GetByIdAsync(request.OwnerId, cancellationToken);

        if (owner is null)
        {
            return Result.Failure(new Error(
                "Owner.NotFound",
                $"Sahib tapılmadı: {request.OwnerId}"));
        }

        if (owner.UserId.HasValue)
        {
            return Result.Failure(new Error(
                "Owner.AlreadyLinked",
                "Bu sahib artıq bir hesaba bağlıdır."));
        }

        var existingLink = await _ownerRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (existingLink is not null)
        {
            return Result.Failure(new Error(
                "Owner.UserAlreadyLinked",
                "Bu istifadəçi artıq başqa bir sahibə bağlıdır."));
        }

        owner.LinkToUser(request.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
