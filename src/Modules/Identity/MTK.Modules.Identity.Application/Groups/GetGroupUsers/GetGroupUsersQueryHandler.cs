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

    public async Task<Result<List<UserDto>>> Handle(GetGroupUsersQuery request, CancellationToken cancellationToken)
    {
        List<Abstractions.UserDto> keycloakMembers = await _authenticationService.GetGroupMembersAsync(request.GroupId, cancellationToken);

        // Map from Keycloak UserDto to local UserDto
        var members = keycloakMembers.Select(u => new UserDto(
            Guid.Parse(u.Id), // Convert string ID to Guid
            u.Email,
            u.FirstName ?? string.Empty,
            u.LastName ?? string.Empty,
            null // PhoneNumber - not provided by Keycloak API
        )).ToList();

        return Result.Success(members);
    }
}
