using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.EmployeeEducationHistories.GetEmployeeEducationHistories;

public sealed record GetEmployeeEducationHistoriesQuery(Guid EmployeeId) : IQuery<GetEmployeeEducationHistoriesResponse>;
