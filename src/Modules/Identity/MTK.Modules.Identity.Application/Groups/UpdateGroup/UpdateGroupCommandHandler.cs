using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Groups;

namespace MTK.Modules.Identity.Application.Groups.UpdateGroup;

internal sealed class UpdateGroupCommandHandler : ICommandHandler<UpdateGroupCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGroupCommandHandler(
        IGroupRepository groupRepository,
        IAuthenticationService authenticationService,
        IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        Group? group = await _groupRepository.GetByIdAsync(request.Id, cancellationToken);

        if (group is null)
        {
            return Result.Failure(GroupErrors.NotFound(request.Id));
        }

        // Update in domain
        group.Update(request.Name, request.Description, request.ParentGroupId);

        _groupRepository.Update(group);

        // Update in Keycloak
        await _authenticationService.UpdateGroupAsync(
            group.KeycloakGroupId,
            request.Name,
            request.Description);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
