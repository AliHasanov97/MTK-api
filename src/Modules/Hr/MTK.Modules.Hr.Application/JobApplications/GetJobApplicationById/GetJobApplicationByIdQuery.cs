using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.JobApplications.GetJobApplicationById;

public sealed record GetJobApplicationByIdQuery(Guid Id) : IQuery<GetJobApplicationByIdResponse>;
