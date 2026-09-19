using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Groups;

namespace MTK.Modules.Identity.Application.Groups.DeleteGroup;

internal sealed class DeleteGroupCommandHandler : ICommandHandler<DeleteGroupCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGroupCommandHandler(
        IGroupRepository groupRepository,
        IAuthenticationService authenticationService,
        IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        Group? group = await _groupRepository.GetByIdAsync(request.Id, cancellationToken);

        if (group is null)
        {
            return Result.Failure(GroupErrors.NotFound(request.Id));
        }

        // Soft delete in domain
        _groupRepository.Remove(group);

        // Delete from Keycloak
        await _authenticationService.DeleteGroupAsync(group.KeycloakGroupId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
