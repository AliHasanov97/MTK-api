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

        List<Abstractions.GroupDto> groups = await _authenticationService.GetUserGroupsAsync(user.IdentityId, cancellationToken);

        var response = new UserGroupsResponse(request.UserId, groups);

        return Result.Success(response);
    }
}
