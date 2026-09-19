using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.GetGroupById;

public sealed record GetGroupByIdQuery(Guid Id) : IQuery<GroupDetailResponse>;
