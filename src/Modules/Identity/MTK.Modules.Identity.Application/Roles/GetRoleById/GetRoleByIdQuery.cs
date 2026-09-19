using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Roles.GetRoleById;

public sealed record GetRoleByIdQuery(Guid Id) : IQuery<RoleDetailResponse>;
