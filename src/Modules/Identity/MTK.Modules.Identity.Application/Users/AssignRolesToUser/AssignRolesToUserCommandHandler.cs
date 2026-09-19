using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Roles;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.AssignRolesToUser;

internal sealed class AssignRolesToUserCommandHandler : ICommandHandler<AssignRolesToUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRolesToUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IAuthenticationService authenticationService,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _authenticationService = authenticationService;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
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

        // Assign roles in database
        await _userRepository.AssignRolesToUserAsync(
            request.UserId,
            roleIds,
            _userContext.UserId,
            cancellationToken);

        // Assign roles in Keycloak
        await _authenticationService.AssignRealmRolesToUserAsync(
            user.IdentityId,
            request.RoleNames);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
