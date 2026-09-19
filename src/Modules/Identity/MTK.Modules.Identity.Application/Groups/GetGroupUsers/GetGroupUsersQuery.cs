using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.GetGroupUsers;

public sealed record GetGroupUsersQuery(Guid GroupId) : IQuery<List<UserDto>>;
