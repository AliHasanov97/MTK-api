using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.GetUserRolesAndGroups;

internal sealed class GetUserRolesAndGroupsQueryHandler : IQueryHandler<GetUserRolesAndGroupsQuery, UserRolesAndGroupsResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;

    public GetUserRolesAndGroupsQueryHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
    }

    public async Task<Result<UserRolesAndGroupsResponse>> Handle(GetUserRolesAndGroupsQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserRolesAndGroupsResponse>(UserErrors.NotFound(request.UserId));
        }

        // Get direct roles from Keycloak
        var directRoleNames = await _authenticationService.GetUserDirectRoleNamesAsync(user.IdentityId, cancellationToken);
        var directRoleInfos = directRoleNames.Select(name => new RoleInfo(
            Guid.Empty,
            name,
            string.Empty,
            "Realm")).ToList();

        // Inherited roles (Keycloak does not provide this separately)
        var inheritedRoleInfos = new List<RoleInfo>();

        // Groups (not supported with current IAuthenticationService)
        var groupInfos = new List<GroupInfo>();

        var response = new UserRolesAndGroupsResponse(
            request.UserId,
            directRoleInfos,
            inheritedRoleInfos,
            groupInfos);

        return Result.Success(response);
    }
}
