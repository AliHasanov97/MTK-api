using MTK.Common.Application.Messaging;
using MediatR;

namespace MTK.Modules.Hr.Application.JobApplications.DeleteJobApplication;

public sealed record DeleteJobApplicationCommand(Guid Id) : ICommand<Unit>;
