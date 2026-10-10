using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories.GetEmployeeWorkHistories;

public sealed record GetEmployeeWorkHistoriesQuery(Guid EmployeeId) : IQuery<GetEmployeeWorkHistoriesResponse>;
