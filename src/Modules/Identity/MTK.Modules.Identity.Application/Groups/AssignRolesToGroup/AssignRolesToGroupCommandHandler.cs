using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Groups;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Application.Groups.AssignRolesToGroup;

internal sealed class AssignRolesToGroupCommandHandler : ICommandHandler<AssignRolesToGroupCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRolesToGroupCommandHandler(
        IGroupRepository groupRepository,
        IRoleRepository roleRepository,
        IAuthenticationService authenticationService,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
        _roleRepository = roleRepository;
        _authenticationService = authenticationService;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignRolesToGroupCommand request, CancellationToken cancellationToken)
    {
        Group? group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            return Result.Failure(GroupErrors.NotFound(request.GroupId));
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
        await _groupRepository.AssignRolesToGroupAsync(
            request.GroupId,
            roleIds,
            _userContext.UserId,
            cancellationToken);

        // Assign roles in Keycloak
        await _authenticationService.AssignRolesToGroupAsync(
            group.KeycloakGroupId,
            request.RoleNames);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
