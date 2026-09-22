using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.AssignRolesToUser;

internal sealed class AssignRolesToUserCommandHandler : ICommandHandler<AssignRolesToUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;

    public AssignRolesToUserCommandHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
    {
        // Get user to retrieve IdentityId
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        // Assign roles directly in Keycloak
        await _authenticationService.AssignRealmRolesToUserAsync(
            user.IdentityId,
            request.RoleNames,
            cancellationToken);

        return Result.Success();
    }
}
