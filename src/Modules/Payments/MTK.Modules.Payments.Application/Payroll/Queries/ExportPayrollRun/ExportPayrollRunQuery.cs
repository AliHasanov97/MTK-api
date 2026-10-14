using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payroll.Queries.ExportPayrollRun;

/// <summary>Bir PayrollRun-dakı bütün işçilərin qəbzini bir PDF-də (hər biri öz səhifəsində) ixrac edir.</summary>
public sealed record ExportPayrollRunQuery(Guid PayrollRunId) : IQuery<ExportFileResult>;
