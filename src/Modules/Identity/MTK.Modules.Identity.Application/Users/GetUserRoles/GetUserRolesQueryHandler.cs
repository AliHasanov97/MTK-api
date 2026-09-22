using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.GetUserRoles;

internal sealed class GetUserRolesQueryHandler : IQueryHandler<GetUserRolesQuery, UserRolesResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;

    public GetUserRolesQueryHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
    }

    public async Task<Result<UserRolesResponse>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserRolesResponse>(UserErrors.NotFound(request.UserId));
        }

        // Get direct roles from Keycloak
        var directRoleNames = await _authenticationService.GetUserDirectRoleNamesAsync(user.IdentityId, cancellationToken);
        var directRoleDtos = directRoleNames.Select(name => new RoleDto(
            Guid.Empty,
            name,
            string.Empty,
            "Realm")).ToList();

        List<RoleDto>? inheritedRoleDtos = null;

        if (request.IncludeInheritedRoles)
        {
            // Keycloak does not provide inherited roles separately
            // This would require getting group roles and combining them
            inheritedRoleDtos = new List<RoleDto>();
        }

        var response = new UserRolesResponse(
            request.UserId,
            directRoleDtos,
            inheritedRoleDtos);

        return Result.Success(response);
    }
}
