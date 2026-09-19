using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.GetGroupRoleNames;

public sealed record GetGroupRoleNamesQuery() : IQuery<List<string>>;
