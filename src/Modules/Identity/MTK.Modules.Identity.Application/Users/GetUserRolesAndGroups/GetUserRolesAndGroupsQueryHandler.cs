using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.GetUserRolesAndGroups;

internal sealed class GetUserRolesAndGroupsQueryHandler : IQueryHandler<GetUserRolesAndGroupsQuery, UserRolesAndGroupsResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserRolesAndGroupsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserRolesAndGroupsResponse>> Handle(GetUserRolesAndGroupsQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserRolesAndGroupsResponse>(UserErrors.NotFound(request.UserId));
        }

        // Get direct roles
        var directRoles = await _userRepository.GetUserDirectRolesAsync(request.UserId, cancellationToken);
        var directRoleInfos = directRoles.Select(r => new RoleInfo(
            r.Id,
            r.Name,
            r.Description,
            r.RoleType.ToString())).ToList();

        // Get effective roles (direct + inherited)
        var effectiveRoles = await _userRepository.GetUserEffectiveRolesAsync(request.UserId, cancellationToken);

        // Inherited roles = effective roles - direct roles
        var inheritedRoles = effectiveRoles
            .Where(er => !directRoles.Any(dr => dr.Id == er.Id))
            .ToList();

        var inheritedRoleInfos = inheritedRoles.Select(r => new RoleInfo(
            r.Id,
            r.Name,
            r.Description,
            r.RoleType.ToString())).ToList();

        // Get groups
        var groups = await _userRepository.GetUserGroupsAsync(request.UserId, cancellationToken);
        var groupInfos = groups.Select(g => new GroupInfo(
            g.Id,
            g.KeycloakGroupId,
            g.Name,
            g.Description)).ToList();

        var response = new UserRolesAndGroupsResponse(
            request.UserId,
            directRoleInfos,
            inheritedRoleInfos,
            groupInfos);

        return Result.Success(response);
    }
}
