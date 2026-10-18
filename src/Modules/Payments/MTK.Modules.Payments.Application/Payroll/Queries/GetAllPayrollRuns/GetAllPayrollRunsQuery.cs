using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payroll.Queries.GetAllPayrollRuns;

public sealed record GetAllPayrollRunsQuery()
    : IQuery<IEnumerable<PayrollRunListItemResponse>>;
