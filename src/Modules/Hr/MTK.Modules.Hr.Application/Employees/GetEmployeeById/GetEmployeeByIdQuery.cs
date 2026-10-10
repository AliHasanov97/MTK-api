using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.Employees.GetEmployeeById;

public sealed record GetEmployeeByIdQuery(Guid Id) : IQuery<GetEmployeeByIdResponse>;
