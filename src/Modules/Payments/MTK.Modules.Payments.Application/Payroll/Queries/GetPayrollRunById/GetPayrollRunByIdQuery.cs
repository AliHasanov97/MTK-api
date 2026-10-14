using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payroll;

namespace MTK.Modules.Payments.Application.Payroll.Queries.GetPayrollRunById;

public sealed record GetPayrollRunByIdQuery(Guid Id) : IQuery<PayrollRunResponse>;
