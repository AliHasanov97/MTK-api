using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.GetGroupRoles;

internal sealed class GetGroupRolesQueryHandler : IQueryHandler<GetGroupRolesQuery, List<RoleDto>>
{
    private readonly IAuthenticationService _authenticationService;

    public GetGroupRolesQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<List<RoleDto>>> Handle(GetGroupRolesQuery request, CancellationToken cancellationToken)
    {
        var roleNames = await _authenticationService.GetGroupRoleNamesAsync(request.GroupId, cancellationToken);

        var roleDtos = roleNames.Select(name => new RoleDto(
            Guid.Empty,
            name,
            string.Empty,
            "Realm")).ToList();

        return Result.Success(roleDtos);
    }
}
