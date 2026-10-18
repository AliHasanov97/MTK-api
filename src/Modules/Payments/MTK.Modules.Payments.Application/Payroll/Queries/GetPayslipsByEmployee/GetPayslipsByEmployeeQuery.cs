using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payroll;

namespace MTK.Modules.Payments.Application.Payroll.Queries.GetPayslipsByEmployee;

public sealed record GetPayslipsByEmployeeQuery(Guid EmployeeId)
    : IQuery<IEnumerable<EmployeePayslipResponse>>;
