using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.GetUserGroups;

public sealed record GetUserGroupsQuery(Guid UserId) : IQuery<UserGroupsResponse>;
