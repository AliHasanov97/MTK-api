using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.GetGroupRoleNames;

internal sealed class GetGroupRoleNamesQueryHandler : IQueryHandler<GetGroupRoleNamesQuery, List<string>>
{
    private readonly IAuthenticationService _authenticationService;

    public GetGroupRoleNamesQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<List<string>>> Handle(GetGroupRoleNamesQuery request, CancellationToken cancellationToken)
    {
        var roleNames = await _authenticationService.GetRealmRoleNamesAsync(cancellationToken);

        return Result.Success(roleNames);
    }
}
