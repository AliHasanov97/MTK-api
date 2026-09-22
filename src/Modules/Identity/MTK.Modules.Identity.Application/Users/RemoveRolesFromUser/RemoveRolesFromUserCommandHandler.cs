using MTK.Common.Domain.Abstractions;
using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.RemoveRolesFromUser;

internal sealed class RemoveRolesFromUserCommandHandler : ICommandHandler<RemoveRolesFromUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;

    public RemoveRolesFromUserCommandHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(RemoveRolesFromUserCommand request, CancellationToken cancellationToken)
    {
        // Get user to retrieve IdentityId
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        // Remove roles directly from Keycloak
        await _authenticationService.RemoveRealmRolesFromUserAsync(
            user.IdentityId,
            request.RoleNames,
            cancellationToken);

        return Result.Success();
    }
}
