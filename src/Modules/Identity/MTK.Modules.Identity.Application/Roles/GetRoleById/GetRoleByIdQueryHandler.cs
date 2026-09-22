using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Roles.GetRoleById;

internal sealed class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, RoleDetailResponse>
{
    private readonly IAuthenticationService _authenticationService;

    public GetRoleByIdQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<RoleDetailResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var roleNames = await _authenticationService.GetRealmRoleNamesAsync(cancellationToken);

        // ID ilə role tapmaq mümkün olmadığı üçün, error qaytarırıq
        return Result.Failure<RoleDetailResponse>(
            new Error("Role.NotFoundById", "Role lookup by ID is not supported with Keycloak. Use role name instead."));
    }
}
