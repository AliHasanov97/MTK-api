using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.Jobs.GetJobById;

public sealed record GetJobByIdQuery(Guid Id) : IQuery<GetJobByIdResponse>;
