using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Groups;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Groups.AddUserToGroup;

internal sealed class AddUserToGroupCommandHandler : ICommandHandler<AddUserToGroupCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public AddUserToGroupCommandHandler(
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        IAuthenticationService authenticationService,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _authenticationService = authenticationService;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddUserToGroupCommand request, CancellationToken cancellationToken)
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

        // Add to database
        await _groupRepository.AddUserToGroupAsync(
            request.GroupId,
            request.UserId,
            _userContext.UserId,
            cancellationToken);

        // Add to Keycloak
        await _authenticationService.AddUserToGroupAsync(
            user.IdentityId,
            group.KeycloakGroupId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
