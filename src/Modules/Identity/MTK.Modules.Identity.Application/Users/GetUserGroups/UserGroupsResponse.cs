using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Users.GetUserGroups;

public sealed record UserGroupsResponse(
    Guid UserId,
    List<GroupDto> Groups);
