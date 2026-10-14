using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payroll.Queries.ExportPayslip;

/// <summary>Bir PayrollRun daxilində bir işçinin qəbzini PDF kimi ixrac edir.</summary>
public sealed record ExportPayslipQuery(Guid PayrollRunId, Guid EmployeeId) : IQuery<ExportFileResult>;
