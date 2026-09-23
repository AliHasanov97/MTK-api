using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.GetGroupUsers;

public sealed record GetGroupUsersQuery(Guid GroupId) : IQuery<List<UserDto>>;
