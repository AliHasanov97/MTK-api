using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.GetGroups;

public sealed record GetGroupsQuery() : IQuery<List<GroupResponse>>;
