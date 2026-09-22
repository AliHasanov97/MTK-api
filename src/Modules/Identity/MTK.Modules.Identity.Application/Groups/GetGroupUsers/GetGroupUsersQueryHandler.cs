using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.GetGroupUsers;

internal sealed class GetGroupUsersQueryHandler : IQueryHandler<GetGroupUsersQuery, List<UserDto>>
{
    private readonly IAuthenticationService _authenticationService;

    public GetGroupUsersQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public Task<Result<List<UserDto>>> Handle(GetGroupUsersQuery request, CancellationToken cancellationToken)
    {
        // Keycloak does not provide a direct method to get group members via IAuthenticationService
        // This requires additional implementation in the service
        return Task.FromResult(Result.Failure<List<UserDto>>(
            new Error("Group.GetMembersNotSupported",
                "Group members lookup is not supported with current Keycloak integration. Additional API methods required.")));
    }
}
