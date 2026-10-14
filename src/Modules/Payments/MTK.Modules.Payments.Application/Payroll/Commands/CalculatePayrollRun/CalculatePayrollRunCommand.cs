using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payroll;

namespace MTK.Modules.Payments.Application.Payroll.Commands.CalculatePayrollRun;

/// <summary>Verilmiş ay üçün bütün (sinxronlaşmış) işçilər üzrə əmək haqqı hesablamasını işə salır.</summary>
public sealed record CalculatePayrollRunCommand(int Year, int Month) : ICommand<PayrollRunResponse>;
