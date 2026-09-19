using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Application.Roles.GetRoleById;

namespace MTK.Modules.Identity.Application.Roles.GetRoleByName;

public sealed record GetRoleByNameQuery(string Name) : IQuery<RoleDetailResponse>;
