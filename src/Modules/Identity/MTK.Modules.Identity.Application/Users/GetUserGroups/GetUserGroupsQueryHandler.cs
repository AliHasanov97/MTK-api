using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.GetUserGroups;

internal sealed class GetUserGroupsQueryHandler : IQueryHandler<GetUserGroupsQuery, UserGroupsResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserGroupsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserGroupsResponse>> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserGroupsResponse>(UserErrors.NotFound(request.UserId));
        }

        var groups = await _userRepository.GetUserGroupsAsync(request.UserId, cancellationToken);

        var groupDtos = groups.Select(g => new GroupDto(
            g.Id,
            g.KeycloakGroupId,
            g.Name,
            g.Description,
            g.ParentGroupId)).ToList();

        var response = new UserGroupsResponse(request.UserId, groupDtos);

        return Result.Success(response);
    }
}
