using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.GetGroupRoles;

public sealed record GetGroupRolesQuery(Guid GroupId) : IQuery<List<RoleDto>>;
