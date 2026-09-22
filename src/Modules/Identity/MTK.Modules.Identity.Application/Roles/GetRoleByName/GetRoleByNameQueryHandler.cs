using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Application.Roles.GetRoleById;

namespace MTK.Modules.Identity.Application.Roles.GetRoleByName;

internal sealed class GetRoleByNameQueryHandler : IQueryHandler<GetRoleByNameQuery, RoleDetailResponse>
{
    private readonly IAuthenticationService _authenticationService;

    public GetRoleByNameQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<RoleDetailResponse>> Handle(GetRoleByNameQuery request, CancellationToken cancellationToken)
    {
        var roleNames = await _authenticationService.GetRealmRoleNamesAsync(cancellationToken);

        if (!roleNames.Contains(request.Name))
        {
            return Result.Failure<RoleDetailResponse>(
                new Error("Role.NotFoundByName", $"Role with name '{request.Name}' was not found."));
        }

        var response = new RoleDetailResponse(
            Guid.Empty,
            request.Name,
            string.Empty,
            "Realm",
            true,
            DateTime.UtcNow,
            DateTime.UtcNow);

        return Result.Success(response);
    }
}
