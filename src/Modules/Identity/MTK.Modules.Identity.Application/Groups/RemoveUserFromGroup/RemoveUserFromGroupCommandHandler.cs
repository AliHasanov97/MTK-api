using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Groups;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Groups.RemoveUserFromGroup;

internal sealed class RemoveUserFromGroupCommandHandler : ICommandHandler<RemoveUserFromGroupCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveUserFromGroupCommandHandler(
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        IAuthenticationService authenticationService,
        IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveUserFromGroupCommand request, CancellationToken cancellationToken)
    {
        Group? group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            return Result.Failure(GroupErrors.NotFound(request.GroupId));
        }

        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        // Remove from database
        await _groupRepository.RemoveUserFromGroupAsync(
            request.GroupId,
            request.UserId,
            cancellationToken);

        // Remove from Keycloak
        await _authenticationService.RemoveUserFromGroupAsync(
            user.IdentityId,
            group.KeycloakGroupId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
