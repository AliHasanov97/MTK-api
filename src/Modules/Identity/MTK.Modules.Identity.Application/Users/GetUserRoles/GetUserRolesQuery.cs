using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.GetUserRoles;

public sealed record GetUserRolesQuery(
    Guid UserId,
    bool IncludeInheritedRoles) : IQuery<UserRolesResponse>;
