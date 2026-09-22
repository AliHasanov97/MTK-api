using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Roles.GetAllRoles;

internal sealed class GetAllRolesQueryHandler : IQueryHandler<GetAllRolesQuery, List<RoleResponse>>
{
    private readonly IAuthenticationService _authenticationService;

    public GetAllRolesQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<List<RoleResponse>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roleNames = await _authenticationService.GetRealmRoleNamesAsync(cancellationToken);

        var response = roleNames.Select(name => new RoleResponse(
            Guid.Empty,
            name,
            string.Empty,
            "Realm",
            true)).ToList();

        return Result.Success(response);
    }
}
