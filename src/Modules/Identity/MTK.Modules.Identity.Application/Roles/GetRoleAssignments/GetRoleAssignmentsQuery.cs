using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Roles.GetRoleAssignments;

public sealed record GetRoleAssignmentsQuery(Guid Id) : IQuery<RoleAssignmentsResponse>;
