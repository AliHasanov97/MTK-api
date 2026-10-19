using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Appeals.Queries.GetAppealById;

public sealed record GetAppealByIdQuery(Guid AppealId) : IQuery<AppealDetailResponse>;
