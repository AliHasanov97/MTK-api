using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payroll;

namespace MTK.Modules.Payments.Application.Payroll.Commands.ApprovePayrollRun;

/// <summary>Hesablamanın rəqəmlərini təsdiqləyir — ödəniş yalnız bundan sonra mümkündür.</summary>
public sealed record ApprovePayrollRunCommand(Guid Id) : ICommand<PayrollRunResponse>;
