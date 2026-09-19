using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Roles.GetAllRoles;

public sealed record GetAllRolesQuery() : IQuery<List<RoleResponse>>;
