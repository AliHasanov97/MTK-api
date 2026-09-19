namespace MTK.Modules.Identity.Application.Users.GetUserGroups;

public sealed record UserGroupsResponse(
    Guid UserId,
    List<GroupDto> Groups);

public sealed record GroupDto(
    Guid Id,
    Guid KeycloakGroupId,
    string Name,
    string? Description,
    Guid? ParentGroupId);
