using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Owners;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Owners.Commands.SyncOwner;

internal sealed class SyncOwnerCommandHandler : ICommandHandler<SyncOwnerCommand>
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncOwnerCommandHandler(IOwnerRepository ownerRepository, IUnitOfWork unitOfWork)
    {
        _ownerRepository = ownerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SyncOwnerCommand request, CancellationToken cancellationToken)
    {
        var owner = await _ownerRepository.GetByIdDefaultAsync(request.OwnerId, cancellationToken);
        if (owner is not null)
        {
            owner.UpdateFullName(request.FullName);
        }
        else
        {
            owner = Owner.Create(request.OwnerId, request.FullName);
            _ownerRepository.Add(owner);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
