using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.GetUserGroups;

internal sealed class GetUserGroupsQueryHandler : IQueryHandler<GetUserGroupsQuery, UserGroupsResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;

    public GetUserGroupsQueryHandler(
        IUserRepository userRepository,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
    }

    public async Task<Result<UserGroupsResponse>> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserGroupsResponse>(UserErrors.NotFound(request.UserId));
        }

        List<Abstractions.GroupDto> keycloakGroups = await _authenticationService.GetUserGroupsAsync(user.IdentityId, cancellationToken);

        // Map to response DTOs
        var groupDtos = keycloakGroups.Select(g => new GroupDto(
            g.Id,
            g.Id, // KeycloakGroupId is same as Id from Keycloak
            g.Name,
            g.Description,
            null // ParentGroupId - not provided by Keycloak API
        )).ToList();

        var response = new UserGroupsResponse(request.UserId, groupDtos);

        return Result.Success(response);
    }
}
