using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payroll.Services;

namespace MTK.Modules.Payments.Application.Payroll.Commands.RecalculatePayrollRun;

internal sealed class RecalculatePayrollRunCommandHandler
    : ICommandHandler<RecalculatePayrollRunCommand, PayrollRunResponse>
{
    private readonly IPayrollRunGenerationService _payrollRunGenerationService;

    public RecalculatePayrollRunCommandHandler(IPayrollRunGenerationService payrollRunGenerationService)
    {
        _payrollRunGenerationService = payrollRunGenerationService;
    }

    public Task<Result<PayrollRunResponse>> Handle(
        RecalculatePayrollRunCommand request,
        CancellationToken cancellationToken)
    {
        return _payrollRunGenerationService.RecalculateAsync(request.PayrollRunId, cancellationToken);
    }
}
