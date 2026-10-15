using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payroll;

namespace MTK.Modules.Payments.Application.Payroll.Queries.GetPayrollRunByPeriod;

public sealed record GetPayrollRunByPeriodQuery(int Year, int Month) : IQuery<PayrollRunResponse>;
