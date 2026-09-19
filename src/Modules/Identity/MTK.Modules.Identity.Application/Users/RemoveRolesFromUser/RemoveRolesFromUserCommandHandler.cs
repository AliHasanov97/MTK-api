using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Roles;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.RemoveRolesFromUser;

internal sealed class RemoveRolesFromUserCommandHandler : ICommandHandler<RemoveRolesFromUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRolesFromUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IAuthenticationService authenticationService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveRolesFromUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        // Get role IDs from names
        var roleIds = new List<Guid>();
        foreach (var roleName in request.RoleNames)
        {
            Role? role = await _roleRepository.GetByNameAsync(roleName, cancellationToken);
            if (role is null)
            {
                return Result.Failure(RoleErrors.NotFoundByName(roleName));
            }
            roleIds.Add(role.Id);
        }

        // Remove roles from database
        await _userRepository.RemoveRolesFromUserAsync(
            request.UserId,
            roleIds,
            cancellationToken);

        // Remove roles from Keycloak
        await _authenticationService.RemoveRealmRolesFromUserAsync(
            user.IdentityId,
            request.RoleNames);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
