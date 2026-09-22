using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Roles.GetRoleAssignments;

internal sealed class GetRoleAssignmentsQueryHandler : IQueryHandler<GetRoleAssignmentsQuery, RoleAssignmentsResponse>
{
    private readonly IAuthenticationService _authenticationService;

    public GetRoleAssignmentsQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public Task<Result<RoleAssignmentsResponse>> Handle(GetRoleAssignmentsQuery request, CancellationToken cancellationToken)
    {
        // Keycloak does not support querying role assignments by role ID
        // This functionality requires additional Keycloak API methods
        return Task.FromResult(Result.Failure<RoleAssignmentsResponse>(
            new Error("Role.AssignmentsNotSupported",
                "Role assignments lookup is not supported with direct Keycloak integration. This requires additional API methods.")));
    }
}
