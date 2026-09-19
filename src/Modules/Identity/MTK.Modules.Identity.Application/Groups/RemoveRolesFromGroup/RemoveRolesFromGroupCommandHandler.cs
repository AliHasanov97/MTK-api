using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Groups;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Application.Groups.RemoveRolesFromGroup;

internal sealed class RemoveRolesFromGroupCommandHandler : ICommandHandler<RemoveRolesFromGroupCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRolesFromGroupCommandHandler(
        IGroupRepository groupRepository,
        IRoleRepository roleRepository,
        IAuthenticationService authenticationService,
        IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
        _roleRepository = roleRepository;
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveRolesFromGroupCommand request, CancellationToken cancellationToken)
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

        // Remove roles from database
        await _groupRepository.RemoveRolesFromGroupAsync(
            request.GroupId,
            roleIds,
            cancellationToken);

        // Remove roles from Keycloak
        await _authenticationService.RemoveRolesFromGroupAsync(
            group.KeycloakGroupId,
            request.RoleNames);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
