using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.GetUserRolesAndGroups;

public sealed record GetUserRolesAndGroupsQuery(Guid UserId) : IQuery<UserRolesAndGroupsResponse>;
