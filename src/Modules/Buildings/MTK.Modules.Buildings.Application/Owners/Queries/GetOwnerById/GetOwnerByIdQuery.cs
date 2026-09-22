using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Owners.Queries.GetOwnerById;

public sealed record GetOwnerByIdQuery(Guid OwnerId) : IQuery<OwnerResponse>;
