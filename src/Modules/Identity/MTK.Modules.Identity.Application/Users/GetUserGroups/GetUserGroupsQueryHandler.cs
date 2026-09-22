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

        // Keycloak does not provide a direct method to get user groups via IAuthenticationService
        // This requires additional implementation
        return Result.Failure<UserGroupsResponse>(
            new Error("User.GroupsNotSupported",
                "User groups lookup is not supported with current Keycloak integration. Additional API methods required."));
    }
}
